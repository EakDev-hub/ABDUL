using AbdulBackend.Models;
using Npgsql;

namespace AbdulBackend.Services;

public class AnnouncementService : IAnnouncementService
{
    private readonly string _connectionString;
    private readonly ILogger<AnnouncementService> _logger;

    public AnnouncementService(IConfiguration configuration, ILogger<AnnouncementService> logger)
    {
        // Build connection string from environment variables
        var host = configuration["DB_HOST"] ?? throw new InvalidOperationException("DB_HOST is not configured");
        var port = configuration["DB_PORT"] ?? "5432";
        var database = configuration["DB_NAME"] ?? throw new InvalidOperationException("DB_NAME is not configured");
        var username = configuration["DB_USER"] ?? throw new InvalidOperationException("DB_USER is not configured");
        var password = configuration["DB_PASSWORD"] ?? throw new InvalidOperationException("DB_PASSWORD is not configured");
        
        _connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};Search Path=hackathon";
        _logger = logger;
        
        _logger.LogInformation("Database connection configured for AnnouncementService: Host={Host}, Database={Database}, Schema=hackathon", host, database);
    }

    public async Task<List<Announcement>> GetLatestAnnouncementsAsync()
    {
        var announcements = new List<Announcement>();

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT id, text, posted_at, is_active, created_at, updated_at
                FROM hackathon.announcement
                WHERE is_active = true
                ORDER BY posted_at DESC
                LIMIT 3";
            
            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                announcements.Add(new Announcement
                {
                    Id = reader.GetInt64(0),
                    Text = reader.GetString(1),
                    PostedAt = reader.GetDateTime(2),
                    IsActive = reader.GetBoolean(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5)
                });
            }

            _logger.LogInformation("Successfully retrieved {Count} announcement records", announcements.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving announcement records from database");
            throw;
        }

        return announcements;
    }

    public async Task<List<Announcement>> GetAllAnnouncementsAsync()
    {
        var announcements = new List<Announcement>();

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT id, text, posted_at, is_active, created_at, updated_at
                FROM hackathon.announcement
                ORDER BY posted_at DESC";
            
            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                announcements.Add(new Announcement
                {
                    Id = reader.GetInt64(0),
                    Text = reader.GetString(1),
                    PostedAt = reader.GetDateTime(2),
                    IsActive = reader.GetBoolean(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5)
                });
            }

            _logger.LogInformation("Successfully retrieved {Count} announcement records", announcements.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all announcement records from database");
            throw;
        }

        return announcements;
    }

    public async Task<Announcement?> GetAnnouncementByIdAsync(long id)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT id, text, posted_at, is_active, created_at, updated_at
                FROM hackathon.announcement
                WHERE id = @id";
            
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Announcement
                {
                    Id = reader.GetInt64(0),
                    Text = reader.GetString(1),
                    PostedAt = reader.GetDateTime(2),
                    IsActive = reader.GetBoolean(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5)
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving announcement with id {Id}", id);
            throw;
        }
    }

    public async Task<Announcement> CreateAnnouncementAsync(string text, DateTime postedAt, bool isActive)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO hackathon.announcement (text, posted_at, is_active, created_at, updated_at)
                VALUES (@text, @postedAt, @isActive, @now, @now)
                RETURNING id, text, posted_at, is_active, created_at, updated_at";
            
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@text", text);
            command.Parameters.AddWithValue("@postedAt", postedAt);
            command.Parameters.AddWithValue("@isActive", isActive);
            command.Parameters.AddWithValue("@now", DateTime.UtcNow);
            
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var announcement = new Announcement
                {
                    Id = reader.GetInt64(0),
                    Text = reader.GetString(1),
                    PostedAt = reader.GetDateTime(2),
                    IsActive = reader.GetBoolean(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5)
                };

                _logger.LogInformation("Successfully created announcement with id {Id}", announcement.Id);
                return announcement;
            }

            throw new Exception("Failed to create announcement");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating announcement");
            throw;
        }
    }

    public async Task<Announcement?> UpdateAnnouncementAsync(long id, string? text, DateTime? postedAt, bool? isActive)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var updates = new List<string>();
            var command = new NpgsqlCommand { Connection = connection };
            
            if (text != null)
            {
                updates.Add("text = @text");
                command.Parameters.AddWithValue("@text", text);
            }
            
            if (postedAt.HasValue)
            {
                updates.Add("posted_at = @postedAt");
                command.Parameters.AddWithValue("@postedAt", postedAt.Value);
            }

            if (isActive.HasValue)
            {
                updates.Add("is_active = @isActive");
                command.Parameters.AddWithValue("@isActive", isActive.Value);
            }

            if (updates.Count == 0)
            {
                return await GetAnnouncementByIdAsync(id);
            }

            updates.Add("updated_at = @now");
            command.Parameters.AddWithValue("@now", DateTime.UtcNow);
            command.Parameters.AddWithValue("@id", id);

            command.CommandText = $@"
                UPDATE hackathon.announcement
                SET {string.Join(", ", updates)}
                WHERE id = @id
                RETURNING id, text, posted_at, is_active, created_at, updated_at";
            
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var announcement = new Announcement
                {
                    Id = reader.GetInt64(0),
                    Text = reader.GetString(1),
                    PostedAt = reader.GetDateTime(2),
                    IsActive = reader.GetBoolean(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5)
                };

                _logger.LogInformation("Successfully updated announcement with id {Id}", id);
                return announcement;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating announcement with id {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAnnouncementAsync(long id)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"DELETE FROM hackathon.announcement WHERE id = @id";
            
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            
            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully deleted announcement with id {Id}", id);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting announcement with id {Id}", id);
            throw;
        }
    }
}