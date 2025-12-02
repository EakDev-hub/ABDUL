using AbdulBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbdulBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class QnaController : ControllerBase
{
    private readonly IQnaService _qnaService;
    private readonly ILogger<QnaController> _logger;

    public QnaController(IQnaService qnaService, ILogger<QnaController> logger)
    {
        _qnaService = qnaService;
        _logger = logger;
    }

    /// <summary>
    /// Get latest 3 Q&A records ordered by created_at DESC
    /// </summary>
    /// <returns>List of latest Q&A records</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLatestQna()
    {
        try
        {
            var qnaList = await _qnaService.GetLatestQnaAsync();
            
            return Ok(new
            {
                success = true,
                data = qnaList
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Q&A records");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving Q&A records",
                error = ex.Message
            });
        }
    }
}