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
                LIMIT 5";
            
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

    public async Task<List<Qna>> GetAllQnaAsync()
    {
        var qnaList = new List<Qna>();

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT id, question, answer, created_at, updated_at
                FROM hackathon.qna
                ORDER BY created_at DESC";
            
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
            _logger.LogError(ex, "Error retrieving all Q&A records from database");
            throw;
        }

        return qnaList;
    }

    public async Task<Qna?> GetQnaByIdAsync(long id)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT id, question, answer, created_at, updated_at
                FROM hackathon.qna
                WHERE id = @id";
            
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Qna
                {
                    Id = reader.GetInt64(0),
                    Question = reader.GetString(1),
                    Answer = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3),
                    UpdatedAt = reader.GetDateTime(4)
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Q&A record with id {Id}", id);
            throw;
        }
    }

    public async Task<Qna> CreateQnaAsync(string question, string? answer)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO hackathon.qna (question, answer, created_at, updated_at)
                VALUES (@question, @answer, @now, @now)
                RETURNING id, question, answer, created_at, updated_at";
            
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@question", question);
            command.Parameters.AddWithValue("@answer", (object?)answer ?? DBNull.Value);
            command.Parameters.AddWithValue("@now", DateTime.UtcNow);
            
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var qna = new Qna
                {
                    Id = reader.GetInt64(0),
                    Question = reader.GetString(1),
                    Answer = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3),
                    UpdatedAt = reader.GetDateTime(4)
                };

                _logger.LogInformation("Successfully created Q&A record with id {Id}", qna.Id);
                return qna;
            }

            throw new Exception("Failed to create Q&A record");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Q&A record");
            throw;
        }
    }

    public async Task<Qna?> UpdateQnaAsync(long id, string? question, string? answer)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var updates = new List<string>();
            var command = new NpgsqlCommand { Connection = connection };
            
            if (question != null)
            {
                updates.Add("question = @question");
                command.Parameters.AddWithValue("@question", question);
            }
            
            if (answer != null)
            {
                updates.Add("answer = @answer");
                command.Parameters.AddWithValue("@answer", answer);
            }

            if (updates.Count == 0)
            {
                return await GetQnaByIdAsync(id);
            }

            updates.Add("updated_at = @now");
            command.Parameters.AddWithValue("@now", DateTime.UtcNow);
            command.Parameters.AddWithValue("@id", id);

            command.CommandText = $@"
                UPDATE hackathon.qna
                SET {string.Join(", ", updates)}
                WHERE id = @id
                RETURNING id, question, answer, created_at, updated_at";
            
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var qna = new Qna
                {
                    Id = reader.GetInt64(0),
                    Question = reader.GetString(1),
                    Answer = reader.IsDBNull(2) ? null : reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3),
                    UpdatedAt = reader.GetDateTime(4)
                };

                _logger.LogInformation("Successfully updated Q&A record with id {Id}", id);
                return qna;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Q&A record with id {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteQnaAsync(long id)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"DELETE FROM hackathon.qna WHERE id = @id";
            
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            
            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully deleted Q&A record with id {Id}", id);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Q&A record with id {Id}", id);
            throw;
        }
    }
}