using AbdulBackend.Models;
using AbdulBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbdulBackend.Controllers;

[ApiController]
[Route("api/accuracy")]
public class AccuracyTestController : ControllerBase
{
    private readonly IKnowledgeBaseService _knowledgeBaseService;
    private readonly ILogger<AccuracyTestController> _logger;

    public AccuracyTestController(IKnowledgeBaseService knowledgeBaseService, ILogger<AccuracyTestController> logger)
    {
        _knowledgeBaseService = knowledgeBaseService;
        _logger = logger;
    }

    /// <summary>
    /// Accuracy testing endpoint - Returns simple answer from Knowledge Base
    /// </summary>
    /// <param name="request">Question for testing</param>
    /// <returns>Simple answer response</returns>
    [HttpPost("test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Test([FromBody] AccuracyTestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(new AccuracyTestResponse
            {
                Answer = "Question cannot be empty"
            });
        }

        try
        {
            _logger.LogInformation("Accuracy test question: {Question}", request.Question);

            // Use Knowledge Base to get answer with minimal results for speed
            var kbResponse = await _knowledgeBaseService.RetrieveAndGenerateAsync(
                request.Question,
                modelId: null,
                maxResults: 3
            );

            if (!kbResponse.Success || string.IsNullOrEmpty(kbResponse.Answer))
            {
                var errorMsg = kbResponse.Error ?? "Unknown error";
                _logger.LogWarning("Failed to get answer from Knowledge Base: {Error}", errorMsg);
                return StatusCode(500, new AccuracyTestResponse
                {
                    Answer = $"Unable to retrieve answer: {errorMsg}"
                });
            }

            // Return clean answer
            var answer = kbResponse.Answer.Trim();
            
            _logger.LogInformation("Answer generated: {Answer}", answer);

            return Ok(new AccuracyTestResponse
            {
                Answer = answer
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in accuracy test");
            return StatusCode(500, new AccuracyTestResponse
            {
                Answer = "Error processing question"
            });
        }
    }
}
