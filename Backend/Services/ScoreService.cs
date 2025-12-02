using AbdulBackend.Models;
using Npgsql;

namespace AbdulBackend.Services;

public class ScoreService : IScoreService
{
    private readonly string _connectionString;
    private readonly ILogger<ScoreService> _logger;

    public ScoreService(IConfiguration configuration, ILogger<ScoreService> logger)
    {
        // Build connection string from environment variables
        var host = configuration["DB_HOST"] ?? throw new InvalidOperationException("DB_HOST is not configured");
        var port = configuration["DB_PORT"] ?? "5432";
        var database = configuration["DB_NAME"] ?? throw new InvalidOperationException("DB_NAME is not configured");
        var username = configuration["DB_USER"] ?? throw new InvalidOperationException("DB_USER is not configured");
        var password = configuration["DB_PASSWORD"] ?? throw new InvalidOperationException("DB_PASSWORD is not configured");
        
        _connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};Search Path=hackathon";
        _logger = logger;
        
        _logger.LogInformation("Database connection configured for ScoreService: Host={Host}, Database={Database}, Schema=hackathon", host, database);
    }

    public async Task<List<TeamScoreSummary>> GetTeamScoreSummaryAsync()
    {
        var teamScores = new List<TeamScoreSummary>();

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT DISTINCT ON (team)
                    team,
                    score as total_score,
                    time_used_in_seconds
                FROM hackathon.answer_log
                ORDER BY team, score DESC, time_used_in_seconds ASC";
            
            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                teamScores.Add(new TeamScoreSummary
                {
                    Team = reader.GetString(0),
                    TotalScore = reader.GetDecimal(1),
                    TimeUsedInSeconds = reader.GetDecimal(2)
                });
            }

            // Sort by highest score descending
            teamScores = teamScores.OrderByDescending(t => t.TotalScore).ToList();

            _logger.LogInformation("Successfully retrieved {Count} team score summaries", teamScores.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team score summaries from database");
            throw;
        }

        return teamScores;
    }
}