using AbdulBackend.Models;
using AbdulBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbdulBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DisplayControlController : ControllerBase
{
    private readonly IDisplayControlService _displayControlService;
    private readonly ILogger<DisplayControlController> _logger;

    public DisplayControlController(IDisplayControlService displayControlService, ILogger<DisplayControlController> logger)
    {
        _displayControlService = displayControlService;
        _logger = logger;
    }

    /// <summary>
    /// Get the current display control status (on_stage flag)
    /// </summary>
    /// <returns>Display control record with on_stage boolean</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDisplayControl()
    {
        try
        {
            var displayControl = await _displayControlService.GetDisplayControlAsync();
            
            if (displayControl == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Display control not found. Please initialize it first."
                });
            }

            return Ok(new
            {
                success = true,
                data = displayControl
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get display control");
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to retrieve display control",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Update the display control status (on_stage flag)
    /// </summary>
    /// <param name="dto">Display control update data</param>
    /// <returns>Updated display control record</returns>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateDisplayControl([FromBody] UpdateDisplayControlDto dto)
    {
        try
        {
            var updatedControl = await _displayControlService.UpdateDisplayControlAsync(dto);

            if (updatedControl == null)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to update display control"
                });
            }

            return Ok(new
            {
                success = true,
                message = $"Display control updated: on_stage = {updatedControl.OnStage}",
                data = updatedControl
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating display control");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while updating the display control",
                error = ex.Message
            });
        }
    }
}
