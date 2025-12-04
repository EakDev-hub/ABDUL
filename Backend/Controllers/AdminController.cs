using AbdulBackend.Models;
using AbdulBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbdulBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IQnaService _qnaService;
    private readonly IAnnouncementService _announcementService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IQnaService qnaService, 
        IAnnouncementService announcementService,
        ILogger<AdminController> logger)
    {
        _qnaService = qnaService;
        _announcementService = announcementService;
        _logger = logger;
    }

    #region Q&A Management

    /// <summary>
    /// Get all Q&A records
    /// </summary>
    [HttpGet("qna")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllQna()
    {
        try
        {
            var qnaList = await _qnaService.GetAllQnaAsync();
            
            return Ok(new
            {
                success = true,
                data = qnaList,
                count = qnaList.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all Q&A records");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving Q&A records",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Get Q&A by ID
    /// </summary>
    [HttpGet("qna/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQnaById(long id)
    {
        try
        {
            var qna = await _qnaService.GetQnaByIdAsync(id);
            
            if (qna == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Q&A with ID {id} not found"
                });
            }
            
            return Ok(new
            {
                success = true,
                data = qna
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Q&A with ID {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving Q&A record",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Create new Q&A record
    /// </summary>
    [HttpPost("qna")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateQna([FromBody] CreateQnaDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Question))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Question is required"
                });
            }

            var qna = await _qnaService.CreateQnaAsync(dto.Question, dto.Answer);
            
            return CreatedAtAction(
                nameof(GetQnaById),
                new { id = qna.Id },
                new
                {
                    success = true,
                    message = "Q&A created successfully",
                    data = qna
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Q&A record");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while creating Q&A record",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Update Q&A record
    /// </summary>
    [HttpPut("qna/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateQna(long id, [FromBody] UpdateQnaDto dto)
    {
        try
        {
            var qna = await _qnaService.UpdateQnaAsync(id, dto.Question, dto.Answer);
            
            if (qna == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Q&A with ID {id} not found"
                });
            }
            
            return Ok(new
            {
                success = true,
                message = "Q&A updated successfully",
                data = qna
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Q&A with ID {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while updating Q&A record",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Delete Q&A record
    /// </summary>
    [HttpDelete("qna/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteQna(long id)
    {
        try
        {
            var deleted = await _qnaService.DeleteQnaAsync(id);
            
            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Q&A with ID {id} not found"
                });
            }
            
            return Ok(new
            {
                success = true,
                message = "Q&A deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Q&A with ID {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while deleting Q&A record",
                error = ex.Message
            });
        }
    }

    #endregion

    #region Announcement Management

    /// <summary>
    /// Get all announcements
    /// </summary>
    [HttpGet("announcements")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAnnouncements()
    {
        try
        {
            var announcements = await _announcementService.GetAllAnnouncementsAsync();
            
            return Ok(new
            {
                success = true,
                data = announcements,
                count = announcements.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all announcements");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving announcements",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Get announcement by ID
    /// </summary>
    [HttpGet("announcements/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnnouncementById(long id)
    {
        try
        {
            var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
            
            if (announcement == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Announcement with ID {id} not found"
                });
            }
            
            return Ok(new
            {
                success = true,
                data = announcement
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving announcement with ID {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving announcement",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Create new announcement
    /// </summary>
    [HttpPost("announcements")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Text))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Text is required"
                });
            }

            var postedAt = dto.PostedAt ?? DateTime.UtcNow;
            var announcement = await _announcementService.CreateAnnouncementAsync(
                dto.Text, 
                postedAt, 
                dto.IsActive);
            
            return CreatedAtAction(
                nameof(GetAnnouncementById),
                new { id = announcement.Id },
                new
                {
                    success = true,
                    message = "Announcement created successfully",
                    data = announcement
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating announcement");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while creating announcement",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Update announcement
    /// </summary>
    [HttpPut("announcements/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAnnouncement(long id, [FromBody] UpdateAnnouncementDto dto)
    {
        try
        {
            var announcement = await _announcementService.UpdateAnnouncementAsync(
                id, 
                dto.Text, 
                dto.PostedAt, 
                dto.IsActive);
            
            if (announcement == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Announcement with ID {id} not found"
                });
            }
            
            return Ok(new
            {
                success = true,
                message = "Announcement updated successfully",
                data = announcement
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating announcement with ID {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while updating announcement",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Delete announcement
    /// </summary>
    [HttpDelete("announcements/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAnnouncement(long id)
    {
        try
        {
            var deleted = await _announcementService.DeleteAnnouncementAsync(id);
            
            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Announcement with ID {id} not found"
                });
            }
            
            return Ok(new
            {
                success = true,
                message = "Announcement deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting announcement with ID {Id}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while deleting announcement",
                error = ex.Message
            });
        }
    }

    #endregion
}