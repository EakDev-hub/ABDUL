using AbdulBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbdulBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class AnnouncementController : ControllerBase
{
    private readonly IAnnouncementService _announcementService;
    private readonly ILogger<AnnouncementController> _logger;

    public AnnouncementController(IAnnouncementService announcementService, ILogger<AnnouncementController> logger)
    {
        _announcementService = announcementService;
        _logger = logger;
    }

    /// <summary>
    /// Get latest 5 active announcements ordered by posted_at DESC
    /// </summary>
    /// <returns>List of latest announcements</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLatestAnnouncements()
    {
        try
        {
            var announcements = await _announcementService.GetLatestAnnouncementsAsync();
            
            return Ok(new
            {
                success = true,
                data = announcements
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving announcements");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving announcements",
                error = ex.Message
            });
        }
    }
}