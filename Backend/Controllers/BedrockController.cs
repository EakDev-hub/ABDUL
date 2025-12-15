using AbdulBackend.Models;
using AbdulBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbdulBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class BedrockController : ControllerBase
{
    private readonly IBedrockService _bedrockService;
    private readonly ILogger<BedrockController> _logger;

    public BedrockController(IBedrockService bedrockService, ILogger<BedrockController> logger)
    {
        _bedrockService = bedrockService;
        _logger = logger;
    }

    /// <summary>
    /// Invoke Amazon Bedrock model with a prompt
    /// </summary>
    /// <param name="request">Bedrock request with prompt and optional system prompt</param>
    /// <returns>AI-generated response from Bedrock</returns>
    [HttpPost("invoke")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InvokeModel([FromBody] BedrockRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest(new BedrockResponse
            {
                Success = false,
                Error = "Prompt cannot be empty"
            });
        }

        try
        {
            string response;

            if (!string.IsNullOrWhiteSpace(request.SystemPrompt))
            {
                response = await _bedrockService.InvokeModelWithSystemPromptAsync(
                    request.SystemPrompt,
                    request.Prompt,
                    request.ModelId
                );
            }
            else
            {
                response = await _bedrockService.InvokeModelAsync(request.Prompt, request.ModelId);
            }

            return Ok(new BedrockResponse
            {
                Success = true,
                Response = response,
                ModelId = request.ModelId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invoking Bedrock model");
            return StatusCode(500, new BedrockResponse
            {
                Success = false,
                Error = "An error occurred while invoking the Bedrock model",
                ModelId = request.ModelId
            });
        }
    }

    /// <summary>
    /// Health check endpoint for Bedrock service
    /// </summary>
    /// <returns>Service status</returns>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            status = "healthy",
            service = "Amazon Bedrock",
            message = "Bedrock service is configured and ready"
        });
    }
}
