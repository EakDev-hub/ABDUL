using AbdulBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbdulBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class ScoreController : ControllerBase
{
    private readonly IScoreService _scoreService;
    private readonly ILogger<ScoreController> _logger;

    public ScoreController(IScoreService scoreService, ILogger<ScoreController> logger)
    {
        _scoreService = scoreService;
        _logger = logger;
    }

    /// <summary>
    /// Get team score summary - highest score per team ordered by total score descending
    /// </summary>
    /// <param name="passKeyType">Optional filter by pass key type (e.g., "full", "half")</param>
    /// <returns>List of team scores with highest score per team</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTeamScoreSummary([FromQuery] string? passKeyType = null)
    {
        try
        {
            var teamScores = await _scoreService.GetTeamScoreSummaryAsync(passKeyType);
            
            return Ok(new
            {
                success = true,
                data = teamScores,
                filter = new
                {
                    passKeyType = passKeyType ?? "all"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team score summary with passKeyType filter: {PassKeyType}", passKeyType ?? "None");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving team score summary",
                error = ex.Message
            });
        }
    }
}