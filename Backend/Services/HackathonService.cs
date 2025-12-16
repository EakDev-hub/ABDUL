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
        
        // Phase 3: Create initial answer_log (with is_completed = FALSE)
        var uuid = await CreateInitialAnswerLogAsync(teamPassKey, questions.Count);
        
        // Phase 4: Process questions incrementally with real-time saves
        var (results, timeUsedInSeconds) = await ProcessQuestionsIncrementallyAsync(
            uuid, request.ApiUrl, questions, aiInstruction, teamPassKey.MaxDurationInSeconds);
        
        // Phase 5: Build and return response
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

    private async Task<string> CreateInitialAnswerLogAsync(
        TeamPassKey teamPassKey,
        int totalQuestions)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        const string query = @"
            INSERT INTO hackathon.answer_log
            (team, pass_key, pass_key_type, max_duration_in_seconds,
             total_question, answered_question, maximum_score, score, time_used_in_seconds, is_completed)
            VALUES (@team, @passKey, @passKeyType, @maxDuration,
                    @totalQuestion, 0, @maximumScore, 0, 0, FALSE)
            RETURNING uuid";
        
        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@team", teamPassKey.Team);
        command.Parameters.AddWithValue("@passKey", teamPassKey.PassKey);
        command.Parameters.AddWithValue("@passKeyType", teamPassKey.PassKeyType);
        command.Parameters.AddWithValue("@maxDuration", teamPassKey.MaxDurationInSeconds);
        command.Parameters.AddWithValue("@totalQuestion", totalQuestions);
        command.Parameters.AddWithValue("@maximumScore", (decimal)totalQuestions);
        
        var uuid = (await command.ExecuteScalarAsync())?.ToString()
            ?? throw new InvalidOperationException("Failed to generate UUID");
        
        _logger.LogInformation("Created initial answer_log with UUID: {Uuid}, is_completed = FALSE", uuid);
        return uuid;
    }

    private async Task<(List<QuestionResult>, decimal)> ProcessQuestionsIncrementallyAsync(
        string uuid,
        string apiUrl,
        List<Question> questions,
        AiEvaluationInstruction aiInstruction,
        int maxDurationInSeconds)
    {
        var results = new List<QuestionResult>();
        var stopwatch = new Stopwatch();
        var maxDuration = TimeSpan.FromSeconds(maxDurationInSeconds);
        
        _logger.LogInformation("Starting incremental question processing with {Duration}s time budget", maxDurationInSeconds);
        
        foreach (var question in questions.OrderBy(q => q.No))
        {
            // START/RESUME TIMER
            stopwatch.Start();
            
            // Check time budget
            if (stopwatch.Elapsed >= maxDuration)
            {
                _logger.LogInformation("Time budget exhausted at {Elapsed:F2}s. Answered {Count}/{Total} questions",
                    stopwatch.Elapsed.TotalSeconds, results.Count, questions.Count);
                break;
            }
            
            // Calculate remaining time for this request
            var remainingTime = maxDuration - stopwatch.Elapsed;
            var timeout = TimeSpan.FromSeconds(Math.Min(999, remainingTime.TotalSeconds));
            
            if (timeout.TotalSeconds < 0.1)
            {
                _logger.LogInformation("Insufficient time remaining, stopping at question {No}", question.No);
                break;
            }
                
            // Ask team API (TIMED)
            using var cts = new CancellationTokenSource(timeout);
            string actualAnswer;
            
            try
            {
                actualAnswer = await _teamApiClient.AskQuestionAsync(
                    apiUrl, question.QuestionText, cts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling team API for question {No}", question.No);
                actualAnswer = $"{{\"error\": \"{ex.Message}\"}}";
            }
            
            // PAUSE TIMER (AI evaluation and DB saves are untimed)
            stopwatch.Stop();
            
            _logger.LogInformation("Question {No} answered, elapsed time: {Elapsed:F2}s",
                question.No, stopwatch.Elapsed.TotalSeconds);
            
            // AI Evaluation (UNTIMED)
            decimal score;
            try
            {
                score = await _openRouterClient.EvaluateAnswerAsync(
                    aiInstruction.Instruction,
                    aiInstruction.Model,
                    question.QuestionText,
                    question.ExpectedAnswer,
                    actualAnswer ?? "");
                    
                _logger.LogInformation("Question {No} evaluated: score = {Score}", question.No, score);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating question {No}, setting score to 0", question.No);
                score = 0m;
            }
            
            // Create result object
            var result = new QuestionResult
            {
                No = question.No,
                Question = question.QuestionText,
                ExpectedAnswer = question.ExpectedAnswer,
                ActualAnswer = actualAnswer,
                Score = score
            };
            results.Add(result);
            
            // Save to database immediately (UNTIMED)
            try
            {
                await SaveQuestionResultAsync(uuid, result);
                
                var currentTimeUsed = Math.Round((decimal)stopwatch.Elapsed.TotalSeconds, 2);
                await UpdateAnswerLogProgressAsync(
                    uuid, results.Count, results.Sum(r => r.Score), currentTimeUsed);
                
                _logger.LogInformation(
                    "Question {No} completed and saved: score={Score:F2}, cumulative_score={CumulativeScore:F2}, time={Time:F2}s",
                    question.No, score, results.Sum(r => r.Score), currentTimeUsed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving question {No} to database, continuing...", question.No);
                // Continue processing even if save fails
            }
            
            // Timer will resume in next iteration
        }
        
        var finalTimeUsed = Math.Round(
            (decimal)Math.Min(stopwatch.Elapsed.TotalSeconds, maxDurationInSeconds), 2);
        
        // Mark as completed
        try
        {
            await MarkAnswerLogCompleteAsync(uuid);
            _logger.LogInformation("Processing completed for UUID: {Uuid}, final time: {Time:F2}s, questions: {Count}/{Total}",
                uuid, finalTimeUsed, results.Count, questions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking answer_log as complete for UUID: {Uuid}", uuid);
        }
        
        return (results, finalTimeUsed);
    }

    private async Task SaveQuestionResultAsync(string uuid, QuestionResult result)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        const string query = @"
            INSERT INTO hackathon.answer_log_detail
            (answer_log_uuid, no, question, expected_answer, actual_answer, score)
            VALUES (@uuid, @no, @question, @expectedAnswer, @actualAnswer, @score)";
        
        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@uuid", uuid);
        command.Parameters.AddWithValue("@no", result.No);
        command.Parameters.AddWithValue("@question", result.Question);
        command.Parameters.AddWithValue("@expectedAnswer", result.ExpectedAnswer);
        command.Parameters.AddWithValue("@actualAnswer", (object?)result.ActualAnswer ?? DBNull.Value);
        command.Parameters.AddWithValue("@score", result.Score);
        
        await command.ExecuteNonQueryAsync();
        
        _logger.LogDebug("Saved answer_log_detail for question {No}", result.No);
    }

    private async Task UpdateAnswerLogProgressAsync(
        string uuid,
        int answeredQuestion,
        decimal currentScore,
        decimal timeUsed)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        const string query = @"
            UPDATE hackathon.answer_log
            SET answered_question = @answeredQuestion,
                score = @score,
                time_used_in_seconds = @timeUsed
            WHERE uuid = @uuid";
        
        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@uuid", uuid);
        command.Parameters.AddWithValue("@answeredQuestion", answeredQuestion);
        command.Parameters.AddWithValue("@score", currentScore);
        command.Parameters.AddWithValue("@timeUsed", timeUsed);
        
        await command.ExecuteNonQueryAsync();
        
        _logger.LogDebug(
            "Updated answer_log progress: answered={Answered}, score={Score:F2}, time={Time:F2}s",
            answeredQuestion, currentScore, timeUsed);
    }

    private async Task MarkAnswerLogCompleteAsync(string uuid)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        const string query = @"
            UPDATE hackathon.answer_log
            SET is_completed = TRUE
            WHERE uuid = @uuid";
        
        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@uuid", uuid);
        
        await command.ExecuteNonQueryAsync();
        
        _logger.LogInformation("Marked answer_log as completed: UUID={Uuid}", uuid);
    }

    private HackathonResponse BuildResponse(string uuid, TeamPassKey teamPassKey, int totalQuestions, List<QuestionResult> results, decimal timeUsedInSeconds)
    {
        // Mask expected answers for finalist and present pass key types
        var processedResults = results;
        if (teamPassKey.PassKeyType.Equals("finalist", StringComparison.OrdinalIgnoreCase) ||
            teamPassKey.PassKeyType.Equals("present", StringComparison.OrdinalIgnoreCase))
        {
            processedResults = results.Select(r => new QuestionResult
            {
                No = r.No,
                Question = r.Question,
                ExpectedAnswer = "-",  // Hide expected answer
                ActualAnswer = r.ActualAnswer,
                Score = r.Score
            }).ToList();
            
            _logger.LogInformation("Masked expected answers for PassKeyType: {PassKeyType}", teamPassKey.PassKeyType);
        }
        
        var response = new HackathonResponse
        {
            Uuid = uuid,
            PassKeyType = teamPassKey.PassKeyType,
            MaxDurationInSecs = teamPassKey.MaxDurationInSeconds,
            TimeUsedInSeconds = timeUsedInSeconds,
            TotalQuestion = totalQuestions,
            MaximumScore = totalQuestions,
            AnsweredQuestion = processedResults.Count,
            Score = processedResults.Sum(r => r.Score),
            Results = processedResults
        };

        _logger.LogInformation("Built response: UUID={Uuid}, Time={TimeUsed:F2}s/{Max}s, Answered={Answered}/{Total}, Score={Score:F2}/{Max}",
            uuid, timeUsedInSeconds, teamPassKey.MaxDurationInSeconds, processedResults.Count, totalQuestions, response.Score, totalQuestions);

        return response;
    }
}