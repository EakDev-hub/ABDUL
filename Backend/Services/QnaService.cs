using AbdulBackend.Models;
using Npgsql;

namespace AbdulBackend.Services;

public class QnaService : IQnaService
{
    private readonly string _connectionString;
    private readonly ILogger<QnaService> _logger;

    public QnaService(IConfiguration configuration, ILogger<QnaService> logger)
    {
        // Build connection string from environment variables
        var host = configuration["DB_HOST"] ?? throw new InvalidOperationException("DB_HOST is not configured");
        var port = configuration["DB_PORT"] ?? "5432";
        var database = configuration["DB_NAME"] ?? throw new InvalidOperationException("DB_NAME is not configured");
        var username = configuration["DB_USER"] ?? throw new InvalidOperationException("DB_USER is not configured");
        var password = configuration["DB_PASSWORD"] ?? throw new InvalidOperationException("DB_PASSWORD is not configured");
        
        _connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};Search Path=hackathon";
        _logger = logger;
        
        _logger.LogInformation("Database connection configured for QnaService: Host={Host}, Database={Database}, Schema=hackathon", host, database);
    }

    public async Task<List<Qna>> GetLatestQnaAsync()
    {
        var qnaList = new List<Qna>();

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT id, question, answer, created_at, updated_at 
                FROM hackathon.qna 
                ORDER BY created_at DESC 
                LIMIT 3";
            
            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                qnaList.Add(new Qna
                {
                    Id = reader.GetInt64(0),
                    Question = reader.GetString(1),
                    Answer = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3),
                    UpdatedAt = reader.GetDateTime(4)
                });
            }

            _logger.LogInformation("Successfully retrieved {Count} Q&A records", qnaList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Q&A records from database");
            throw;
        }

        return qnaList;
    }
}