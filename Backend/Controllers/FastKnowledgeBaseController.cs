using AbdulBackend.Models;
using AbdulBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbdulBackend.Controllers;

[ApiController]
[Route("kb")]
public class FastKnowledgeBaseController : ControllerBase
{
    private readonly IKnowledgeBaseService _knowledgeBaseService;
    private readonly ILogger<FastKnowledgeBaseController> _logger;

    public FastKnowledgeBaseController(IKnowledgeBaseService knowledgeBaseService, ILogger<FastKnowledgeBaseController> logger)
    {
        _knowledgeBaseService = knowledgeBaseService;
        _logger = logger;
    }

    /// <summary>
    /// Fast retrieval - Get relevant documents only (no AI generation)
    /// </summary>
    [HttpPost("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FastSearch([FromBody] RetrieveOnlyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(new { success = false, error = "Query cannot be empty" });
        }

        var response = await _knowledgeBaseService.RetrieveAsync(request.Query, request.MaxResults);
        return Ok(response);
    }

    /// <summary>
    /// Full query with AI answer (slower but more complete)
    /// </summary>
    [HttpPost("ask")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Ask([FromBody] KnowledgeBaseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(new { success = false, error = "Query cannot be empty" });
        }

        var response = await _knowledgeBaseService.RetrieveAndGenerateAsync(
            request.Query,
            request.ModelId,
            request.MaxResults
        );
        return Ok(response);
    }
}
