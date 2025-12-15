using AbdulBackend.Models;
using AbdulBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbdulBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class KnowledgeBaseController : ControllerBase
{
    private readonly IKnowledgeBaseService _knowledgeBaseService;
    private readonly ILogger<KnowledgeBaseController> _logger;

    public KnowledgeBaseController(IKnowledgeBaseService knowledgeBaseService, ILogger<KnowledgeBaseController> logger)
    {
        _knowledgeBaseService = knowledgeBaseService;
        _logger = logger;
    }

    /// <summary>
    /// Query the Knowledge Base and get an AI-generated answer with citations
    /// </summary>
    /// <param name="request">Knowledge Base query request</param>
    /// <returns>AI-generated answer with source citations</returns>
    [HttpPost("query")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Query([FromBody] KnowledgeBaseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(new KnowledgeBaseResponse
            {
                Success = false,
                Error = "Query cannot be empty"
            });
        }

        try
        {
            var response = await _knowledgeBaseService.RetrieveAndGenerateAsync(
                request.Query,
                request.ModelId,
                request.MaxResults
            );

            if (!response.Success)
            {
                return StatusCode(500, response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying Knowledge Base");
            return StatusCode(500, new KnowledgeBaseResponse
            {
                Success = false,
                Error = "An error occurred while querying the Knowledge Base"
            });
        }
    }

    /// <summary>
    /// Retrieve relevant documents from Knowledge Base without generating an answer
    /// </summary>
    /// <param name="request">Retrieval request</param>
    /// <returns>List of relevant document chunks</returns>
    [HttpPost("retrieve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Retrieve([FromBody] RetrieveOnlyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(new RetrieveOnlyResponse
            {
                Success = false,
                Error = "Query cannot be empty"
            });
        }

        try
        {
            var response = await _knowledgeBaseService.RetrieveAsync(request.Query, request.MaxResults);

            if (!response.Success)
            {
                return StatusCode(500, response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving from Knowledge Base");
            return StatusCode(500, new RetrieveOnlyResponse
            {
                Success = false,
                Error = "An error occurred while retrieving from the Knowledge Base"
            });
        }
    }

    /// <summary>
    /// Health check endpoint for Knowledge Base service
    /// </summary>
    /// <returns>Service status</returns>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            status = "healthy",
            service = "Amazon Bedrock Knowledge Base",
            message = "Knowledge Base service is configured and ready"
        });
    }
}
