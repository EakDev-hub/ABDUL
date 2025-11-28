using AbdulBackend.Models;
using AbdulBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbdulBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HackathonController : ControllerBase
{
    private readonly IHackathonService _hackathonService;
    private readonly ILogger<HackathonController> _logger;

    public HackathonController(IHackathonService hackathonService, ILogger<HackathonController> logger)
    {
        _hackathonService = hackathonService;
        _logger = logger;
    }

    /// <summary>
    /// Process hackathon submission by calling team's API and evaluating answers with AI
    /// </summary>
    /// <param name="request">Team credentials and API endpoint</param>
    /// <returns>Evaluation results with scores</returns>
    [HttpPost]
    [ProducesResponseType(typeof(HackathonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessHackathon([FromBody] HackathonRequest request)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.Team))
        {
            return BadRequest(new { error = "Invalid request", details = "Field 'team' is required" });
        }

        if (string.IsNullOrWhiteSpace(request.PassKey))
        {
            return BadRequest(new { error = "Invalid request", details = "Field 'passKey' is required" });
        }

        if (string.IsNullOrWhiteSpace(request.ApiUrl))
        {
            return BadRequest(new { error = "Invalid request", details = "Field 'apiUrl' is required" });
        }

        try
        {
            _logger.LogInformation("Received hackathon request from team: {Team}", request.Team);
            
            var response = await _hackathonService.ProcessHackathonAsync(request);
            
            _logger.LogInformation("Successfully processed hackathon for team: {Team}, UUID: {Uuid}", 
                request.Team, response.Uuid);
            
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for passKey");
            return Unauthorized(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation");
            return StatusCode(403, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing hackathon submission for team: {Team}", request.Team);
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }
}