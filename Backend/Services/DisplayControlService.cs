using AbdulBackend.Models;
using Npgsql;

namespace AbdulBackend.Services;

public class DisplayControlService : IDisplayControlService
{
    private readonly string _connectionString;
    private readonly ILogger<DisplayControlService> _logger;

    public DisplayControlService(IConfiguration configuration, ILogger<DisplayControlService> logger)
    {
        // Build connection string from environment variables
        var host = configuration["DB_HOST"] ?? throw new InvalidOperationException("DB_HOST is not configured");
        var port = configuration["DB_PORT"] ?? "5432";
        var database = configuration["DB_NAME"] ?? throw new InvalidOperationException("DB_NAME is not configured");
        var username = configuration["DB_USER"] ?? throw new InvalidOperationException("DB_USER is not configured");
        var password = configuration["DB_PASSWORD"] ?? throw new InvalidOperationException("DB_PASSWORD is not configured");
        
        _connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};Search Path=hackathon";
        _logger = logger;
        
        _logger.LogInformation("DisplayControl service configured: Host={Host}, Database={Database}, Schema=hackathon", host, database);
    }

    public async Task<DisplayControl?> GetDisplayControlAsync()
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Get the first (and should be only) record - table only has on_stage column
            const string query = "SELECT on_stage FROM hackathon.display_control LIMIT 1";
            
            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var displayControl = new DisplayControl
                {
                    OnStage = reader.GetBoolean(0)
                };

                _logger.LogInformation("Successfully retrieved display control: OnStage={OnStage}", displayControl.OnStage);
                return displayControl;
            }

            _logger.LogWarning("No display control record found");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving display control from database");
            throw;
        }
    }

    public async Task<DisplayControl?> UpdateDisplayControlAsync(UpdateDisplayControlDto dto)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Update the record - assuming single row table
            const string updateQuery = @"
                UPDATE hackathon.display_control
                SET on_stage = @on_stage
                RETURNING on_stage";

            await using var command = new NpgsqlCommand(updateQuery, connection);
            command.Parameters.AddWithValue("@on_stage", dto.OnStage);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var updatedControl = new DisplayControl
                {
                    OnStage = reader.GetBoolean(0)
                };

                _logger.LogInformation("Successfully updated display control: OnStage={OnStage}", updatedControl.OnStage);
                return updatedControl;
            }

            _logger.LogWarning("Failed to update display control");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating display control with OnStage: {OnStage}", dto.OnStage);
            throw;
        }
    }
}
