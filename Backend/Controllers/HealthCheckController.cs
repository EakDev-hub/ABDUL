using AbdulBackend.Models;
using AbdulBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbdulBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly IHealthCheckService _healthCheckService;
    private readonly ILogger<HealthCheckController> _logger;

    public HealthCheckController(IHealthCheckService healthCheckService, ILogger<HealthCheckController> logger)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
    }

    /// <summary>
    /// Get all health check records from the database
    /// </summary>
    /// <returns>List of health check records with status</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHealthCheck()
    {
        try
        {
            var healthChecks = await _healthCheckService.GetHealthChecksAsync();
            
            return Ok(new
            {
                status = "healthy",
                data = healthChecks
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new
            {
                status = "unhealthy",
                error = "Database connection failed",
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Update a health check message
    /// </summary>
    /// <param name="dto">Health check update data</param>
    /// <returns>Updated health check record</returns>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateHealthCheck([FromBody] UpdateHealthCheckDto dto)
    {
        if (dto.Id <= 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid ID provided"
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Message))
        {
            return BadRequest(new
            {
                success = false,
                message = "Message cannot be empty"
            });
        }

        try
        {
            var updatedHealthCheck = await _healthCheckService.UpdateHealthCheckAsync(dto);

            if (updatedHealthCheck == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Health check record with ID {dto.Id} not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Health check updated successfully",
                data = updatedHealthCheck
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating health check with ID: {Id}", dto.Id);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while updating the health check",
                error = ex.Message
            });
        }
    }
}