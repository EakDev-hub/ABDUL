using AbdulBackend.Models;
using Npgsql;
using System.Diagnostics;

namespace AbdulBackend.Services;

public class HackathonService : IHackathonService
{
    private readonly string _connectionString;
    private readonly ITeamApiClient _teamApiClient;
    private readonly IOpenRouterClient _openRouterClient;
    private readonly ILogger<HackathonService> _logger;

    public HackathonService(
        IConfiguration configuration,
        ITeamApiClient teamApiClient,
        IOpenRouterClient openRouterClient,
        ILogger<HackathonService> logger)
    {
        // Build connection string from environment variables
        var host = configuration["DB_HOST"] ?? throw new InvalidOperationException("DB_HOST is not configured");
        var port = configuration["DB_PORT"] ?? "5432";
        var database = configuration["DB_NAME"] ?? throw new InvalidOperationException("DB_NAME is not configured");
        var username = configuration["DB_USER"] ?? throw new InvalidOperationException("DB_USER is not configured");
        var password = configuration["DB_PASSWORD"] ?? throw new InvalidOperationException("DB_PASSWORD is not configured");
        
        _connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};Search Path=hackathon";
        _teamApiClient = teamApiClient;
        _openRouterClient = openRouterClient;
        _logger = logger;
        
        _logger.LogInformation("HackathonService initialized with database: {Host}/{Database}", host, database);
    }

    public async Task<HackathonResponse> ProcessHackathonAsync(HackathonRequest request)
    {
        _logger.LogInformation("Processing hackathon submission for team: {Team}", request.Team);

        // Phase 1: Validate and get pass key details
        var teamPassKey = await ValidateAndGetPassKeyAsync(request.PassKey, request.Team);
        
        // Phase 2: Get questions and AI instruction
        var questions = await GetQuestionsAsync(teamPassKey.PassKeyType);
        var aiInstruction = await GetAiInstructionAsync();
        
        // Phase 3: Collect answers from team API (TIME LIMITED)
        var (results, timeUsedInSeconds) = await CollectAnswersFromTeamApiAsync(request.ApiUrl, questions, teamPassKey.MaxDurationInSeconds);
        
        // Phase 4: AI Evaluation (NO TIME LIMIT)
        await EvaluateAnswersWithAiAsync(results, aiInstruction);
        
        // Phase 5: Save to database
        var uuid = await SaveResultsAsync(teamPassKey, questions.Count, results, timeUsedInSeconds);
        
        // Build and return response
        return BuildResponse(uuid, teamPassKey, questions.Count, results, timeUsedInSeconds);
    }

    private async Task<TeamPassKey> ValidateAndGetPassKeyAsync(string passKey, string team)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            SELECT tpk.id, tpk.pass_key, tpk.pass_key_type, tpk.team, pkt.max_duration_in_seconds
            FROM hackathon.team_pass_key tpk
            JOIN hackathon.pass_key_type pkt ON tpk.pass_key_type = pkt.name
            WHERE tpk.pass_key = @passKey";

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@passKey", passKey);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            _logger.LogWarning("Invalid passKey: {PassKey}", passKey);
            throw new UnauthorizedAccessException("Invalid passKey");
        }

        var teamPassKey = new TeamPassKey
        {
            Id = reader.GetInt64(0),
            PassKey = reader.GetString(1),
            PassKeyType = reader.GetString(2),
            Team = reader.GetString(3),
            MaxDurationInSeconds = reader.GetInt32(4)
        };

        if (!teamPassKey.Team.Equals(team, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Team mismatch. Expected: {Expected}, Got: {Got}", teamPassKey.Team, team);
            throw new InvalidOperationException("Team name does not match passKey");
        }

        _logger.LogInformation("Validated passKey for team: {Team}, type: {Type}, duration: {Duration}s", 
            teamPassKey.Team, teamPassKey.PassKeyType, teamPassKey.MaxDurationInSeconds);

        return teamPassKey;
    }

    private async Task<List<Question>> GetQuestionsAsync(string passKeyType)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            SELECT id, pass_key_type, no, question, expected_answer
            FROM hackathon.question
            WHERE pass_key_type = @passKeyType
            ORDER BY no";

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@passKeyType", passKeyType);

        var questions = new List<Question>();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            questions.Add(new Question
            {
                Id = reader.GetInt64(0),
                PassKeyType = reader.GetString(1),
                No = reader.GetInt32(2),
                QuestionText = reader.GetString(3),
                ExpectedAnswer = reader.GetString(4)
            });
        }

        _logger.LogInformation("Retrieved {Count} questions for pass key type: {Type}", questions.Count, passKeyType);
        return questions;
    }

    private async Task<AiEvaluationInstruction> GetAiInstructionAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            SELECT id, name, instruction, model, is_active
            FROM hackathon.ai_evaluation_instruction
            WHERE name = 'answer_similarity_evaluator' AND is_active = true
            LIMIT 1";

        await using var command = new NpgsqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            throw new InvalidOperationException("AI evaluation instruction not found in database");
        }

        var instruction = new AiEvaluationInstruction
        {
            Id = reader.GetInt64(0),
            Name = reader.GetString(1),
            Instruction = reader.GetString(2),
            Model = reader.GetString(3),
            IsActive = reader.GetBoolean(4)
        };

        _logger.LogInformation("Retrieved AI instruction: {Name}, model: {Model}", instruction.Name, instruction.Model);
        return instruction;
    }

    private async Task<(List<QuestionResult>, decimal)> CollectAnswersFromTeamApiAsync(
        string apiUrl,
        List<Question> questions,
        int maxDurationInSeconds)
    {
        var results = new List<QuestionResult>();
        var stopwatch = Stopwatch.StartNew();
        var maxDuration = TimeSpan.FromSeconds(maxDurationInSeconds);

        _logger.LogInformation("Starting question collection with {Duration}s time budget", maxDurationInSeconds);

        foreach (var question in questions.OrderBy(q => q.No))
        {
            if (stopwatch.Elapsed >= maxDuration)
            {
                _logger.LogInformation("Time budget exhausted at {Elapsed:F2}s. Answered {Count}/{Total} questions",
                    stopwatch.Elapsed.TotalSeconds, results.Count, questions.Count);
                break;
            }

            // Calculate remaining time for this request
            var remainingTime = maxDuration - stopwatch.Elapsed;
            var timeoutForThisRequest = TimeSpan.FromSeconds(Math.Min(999, remainingTime.TotalSeconds));

            if (timeoutForThisRequest.TotalSeconds < 0.1)
            {
                _logger.LogInformation("Insufficient time remaining, stopping at question {No}", question.No);
                break;
            }

            using var cts = new CancellationTokenSource(timeoutForThisRequest);
            
            var actualAnswer = await _teamApiClient.AskQuestionAsync(apiUrl, question.QuestionText, cts.Token);

            results.Add(new QuestionResult
            {
                No = question.No,
                Question = question.QuestionText,
                ExpectedAnswer = question.ExpectedAnswer,
                ActualAnswer = actualAnswer,
                Score = 0 // Will be set in AI evaluation phase
            });

            _logger.LogInformation("Question {No} answered in {Elapsed:F2}s (Total: {Total:F2}s)",
                question.No, stopwatch.Elapsed.TotalSeconds, stopwatch.Elapsed.TotalSeconds);
        }

        stopwatch.Stop();
        
        // Calculate actual time used: minimum of elapsed time or max duration, rounded to 2 decimal places
        var timeUsedInSeconds = Math.Round((decimal)Math.Min(stopwatch.Elapsed.TotalSeconds, maxDurationInSeconds), 2);
        
        _logger.LogInformation("Question collection completed. Time used: {Elapsed:F2}s, Questions answered: {Count}",
            timeUsedInSeconds, results.Count);

        return (results, timeUsedInSeconds);
    }

    private async Task EvaluateAnswersWithAiAsync(List<QuestionResult> results, AiEvaluationInstruction aiInstruction)
    {
        _logger.LogInformation("Starting AI evaluation for {Count} answers with max 10 concurrent calls (NO TIME LIMIT)", results.Count);

        var evaluationStopwatch = Stopwatch.StartNew();
        var completedCount = 0;
        var lockObject = new object();

        // Use SemaphoreSlim to throttle to max 10 concurrent API calls
        using var semaphore = new SemaphoreSlim(10, 10);

        var tasks = results.Select(async result =>
        {
            await semaphore.WaitAsync(); // Wait for available slot
            try
            {
                _logger.LogInformation("Evaluating question {No}...", result.No);
                
                var score = await _openRouterClient.EvaluateAnswerAsync(
                    aiInstruction.Instruction,
                    aiInstruction.Model,
                    result.Question,
                    result.ExpectedAnswer,
                    result.ActualAnswer ?? ""
                );

                result.Score = score;
                
                int completed;
                lock (lockObject)
                {
                    completedCount++;
                    completed = completedCount;
                }
                
                _logger.LogInformation("Question {No} evaluated: score = {Score} ({Completed}/{Total})",
                    result.No, score, completed, results.Count);
            }
            finally
            {
                semaphore.Release(); // Free up slot for next task
            }
        }).ToList();

        await Task.WhenAll(tasks);

        evaluationStopwatch.Stop();
        _logger.LogInformation("AI evaluation completed in {Elapsed:F2}s for {Count} questions",
            evaluationStopwatch.Elapsed.TotalSeconds, results.Count);
    }

    private async Task<string> SaveResultsAsync(TeamPassKey teamPassKey, int totalQuestions, List<QuestionResult> results, decimal timeUsedInSeconds)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // Insert answer_log
            const string insertLogQuery = @"
                INSERT INTO hackathon.answer_log
                (team, pass_key, pass_key_type, max_duration_in_seconds, total_question, answered_question, maximum_score, score, time_used_in_seconds)
                VALUES (@team, @passKey, @passKeyType, @maxDuration, @totalQuestion, @answeredQuestion, @maximumScore, @score, @timeUsed)
                RETURNING uuid";

            await using var logCommand = new NpgsqlCommand(insertLogQuery, connection, transaction);
            logCommand.Parameters.AddWithValue("@team", teamPassKey.Team);
            logCommand.Parameters.AddWithValue("@passKey", teamPassKey.PassKey);
            logCommand.Parameters.AddWithValue("@passKeyType", teamPassKey.PassKeyType);
            logCommand.Parameters.AddWithValue("@maxDuration", teamPassKey.MaxDurationInSeconds);
            logCommand.Parameters.AddWithValue("@totalQuestion", totalQuestions);
            logCommand.Parameters.AddWithValue("@answeredQuestion", results.Count);
            logCommand.Parameters.AddWithValue("@maximumScore", (decimal)totalQuestions);
            logCommand.Parameters.AddWithValue("@score", results.Sum(r => r.Score));
            logCommand.Parameters.AddWithValue("@timeUsed", timeUsedInSeconds);

            var uuid = (await logCommand.ExecuteScalarAsync())?.ToString()
                ?? throw new InvalidOperationException("Failed to generate UUID");

            _logger.LogInformation("Created answer_log with UUID: {Uuid}, Time used: {TimeUsed:F2}s", uuid, timeUsedInSeconds);

            // Insert answer_log_detail for each result
            const string insertDetailQuery = @"
                INSERT INTO hackathon.answer_log_detail 
                (answer_log_uuid, no, question, expected_answer, actual_answer, score)
                VALUES (@uuid, @no, @question, @expectedAnswer, @actualAnswer, @score)";

            foreach (var result in results)
            {
                await using var detailCommand = new NpgsqlCommand(insertDetailQuery, connection, transaction);
                detailCommand.Parameters.AddWithValue("@uuid", uuid);
                detailCommand.Parameters.AddWithValue("@no", result.No);
                detailCommand.Parameters.AddWithValue("@question", result.Question);
                detailCommand.Parameters.AddWithValue("@expectedAnswer", result.ExpectedAnswer);
                detailCommand.Parameters.AddWithValue("@actualAnswer", (object?)result.ActualAnswer ?? DBNull.Value);
                detailCommand.Parameters.AddWithValue("@score", result.Score);

                await detailCommand.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            
            _logger.LogInformation("Saved {Count} answer details for UUID: {Uuid}", results.Count, uuid);
            
            return uuid;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error saving results to database");
            throw;
        }
    }

    private HackathonResponse BuildResponse(string uuid, TeamPassKey teamPassKey, int totalQuestions, List<QuestionResult> results, decimal timeUsedInSeconds)
    {
        var response = new HackathonResponse
        {
            Uuid = uuid,
            PassKeyType = teamPassKey.PassKeyType,
            MaxDurationInSecs = teamPassKey.MaxDurationInSeconds,
            TimeUsedInSeconds = timeUsedInSeconds,
            TotalQuestion = totalQuestions,
            MaximumScore = totalQuestions,
            AnsweredQuestion = results.Count,
            Score = results.Sum(r => r.Score),
            Results = results
        };

        _logger.LogInformation("Built response: UUID={Uuid}, Time={TimeUsed:F2}s/{Max}s, Answered={Answered}/{Total}, Score={Score:F2}/{Max}",
            uuid, timeUsedInSeconds, teamPassKey.MaxDurationInSeconds, results.Count, totalQuestions, response.Score, totalQuestions);

        return response;
    }
}