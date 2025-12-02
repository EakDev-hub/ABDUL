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
                LIMIT 5";
            
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
}