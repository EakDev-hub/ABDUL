using AbdulBackend.Models;
using Npgsql;

namespace AbdulBackend.Services;

public class HealthCheckService : IHealthCheckService
{
    private readonly string _connectionString;
    private readonly ILogger<HealthCheckService> _logger;

    public HealthCheckService(IConfiguration configuration, ILogger<HealthCheckService> logger)
    {
        // Build connection string from environment variables
        var host = configuration["DB_HOST"] ?? throw new InvalidOperationException("DB_HOST is not configured");
        var port = configuration["DB_PORT"] ?? "5432";
        var database = configuration["DB_NAME"] ?? throw new InvalidOperationException("DB_NAME is not configured");
        var username = configuration["DB_USER"] ?? throw new InvalidOperationException("DB_USER is not configured");
        var password = configuration["DB_PASSWORD"] ?? throw new InvalidOperationException("DB_PASSWORD is not configured");
        
        _connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};Search Path=hackathon";
        _logger = logger;
        
        _logger.LogInformation("Database connection configured: Host={Host}, Database={Database}, Schema=hackathon", host, database);
    }

    public async Task<List<HealthCheck>> GetHealthChecksAsync()
    {
        var healthChecks = new List<HealthCheck>();

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT id, message FROM hackathon.healthcheck";
            
            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                healthChecks.Add(new HealthCheck
                {
                    Id = reader.GetInt32(0),
                    Message = reader.GetString(1)
                });
            }

            _logger.LogInformation("Successfully retrieved {Count} health check records", healthChecks.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving health check records from database");
            throw;
        }

        return healthChecks;
    }

    public async Task<HealthCheck?> UpdateHealthCheckAsync(UpdateHealthCheckDto dto)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string updateQuery = @"
                UPDATE hackathon.healthcheck 
                SET message = @message 
                WHERE id = @id
                RETURNING id, message";

            await using var command = new NpgsqlCommand(updateQuery, connection);
            command.Parameters.AddWithValue("@id", dto.Id);
            command.Parameters.AddWithValue("@message", dto.Message);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var updatedHealthCheck = new HealthCheck
                {
                    Id = reader.GetInt32(0),
                    Message = reader.GetString(1)
                };

                _logger.LogInformation("Successfully updated health check record with Id: {Id}", dto.Id);
                return updatedHealthCheck;
            }

            _logger.LogWarning("Health check record with Id: {Id} not found", dto.Id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating health check record with Id: {Id}", dto.Id);
            throw;
        }
    }
}