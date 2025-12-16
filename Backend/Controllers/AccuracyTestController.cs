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
    private const int ParallelBatchSize = 10;

    // AWS Knowledge Base prompt template - MUST include $search_results$ placeholder
    // Optimized for 60-70% accuracy
    private const string DefaultInstruction = @"You are a helpful assistant. Answer the user's question using ONLY the search results below.

Search Results:
$search_results$

Instructions:
1. PRIORITY: Use exact matches from search results first
2. If multiple results match, combine information for comprehensive answer
3. If no exact match: find the closest related information (70%+ similarity threshold)
4. ALWAYS cite which search result you used when possible
5. If search results don't contain the answer, say 'ไม่พบข้อมูลในฐานความรู้' (not found in knowledge base)
6. Include relevant links/references when available
7. Answer in Thai language, be concise and direct
8. Focus on factual accuracy over creative interpretation

Answer the question based on the search results above.";

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

            var answer = await GetAnswerWithRetryAsync(request.Question, request.Instruction);

            if (string.IsNullOrEmpty(answer))
            {
                _logger.LogWarning("Failed to get answer from Knowledge Base");
                return StatusCode(500, new AccuracyTestResponse
                {
                    Answer = "Unable to retrieve answer"
                });
            }

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

    /// <summary>
    /// Batch accuracy testing endpoint - Processes multiple questions in parallel (10 per batch)
    /// </summary>
    /// <param name="request">List of questions for testing</param>
    /// <returns>List of answers</returns>
    [HttpPost("test-batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TestBatch([FromBody] AccuracyTestBatchRequest request)
    {
        if (request.Questions == null || !request.Questions.Any())
        {
            return BadRequest(new { error = "Questions list cannot be empty" });
        }

        try
        {
            _logger.LogInformation("Batch accuracy test with {Count} questions", request.Questions.Count);

            var results = new List<AccuracyTestBatchResult>();
            var totalQuestions = request.Questions.Count;

            // Process in batches of 10
            for (int i = 0; i < totalQuestions; i += ParallelBatchSize)
            {
                var batch = request.Questions.Skip(i).Take(ParallelBatchSize).ToList();
                _logger.LogInformation("Processing batch {BatchNum}/{TotalBatches} ({Count} questions)", 
                    (i / ParallelBatchSize) + 1, 
                    (totalQuestions + ParallelBatchSize - 1) / ParallelBatchSize,
                    batch.Count);

                // Process batch in parallel
                var batchTasks = batch.Select(async (question, index) =>
                {
                    var questionIndex = i + index;
                    try
                    {
                        var answer = await GetAnswerWithRetryAsync(question, request.Instruction);
                        return new AccuracyTestBatchResult
                        {
                            Index = questionIndex,
                            Question = question,
                            Answer = answer ?? "Unable to retrieve answer",
                            Success = !string.IsNullOrEmpty(answer)
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing question {Index}: {Question}", questionIndex, question);
                        return new AccuracyTestBatchResult
                        {
                            Index = questionIndex,
                            Question = question,
                            Answer = $"Error: {ex.Message}",
                            Success = false
                        };
                    }
                });

                var batchResults = await Task.WhenAll(batchTasks);
                results.AddRange(batchResults);
            }

            var successCount = results.Count(r => r.Success);
            _logger.LogInformation("Batch test completed: {Success}/{Total} successful", successCount, totalQuestions);

            return Ok(new AccuracyTestBatchResponse
            {
                Results = results,
                TotalQuestions = totalQuestions,
                SuccessCount = successCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch accuracy test");
            return StatusCode(500, new { error = "Error processing batch test" });
        }
    }

    private async Task<string?> GetAnswerWithRetryAsync(string question, string? instruction = null)
    {
        // Preprocess query for better retrieval (minimal overhead)
        var processedQuestion = PreprocessQuery(question);
        
        // Use provided instruction or default (optimized: avoid string allocation if using default)
        var finalInstruction = instruction ?? DefaultInstruction;

        // OPTIMIZED FOR SPEED: Reduced chunks, optimal temperature
        // Use maxResults = 50 for faster retrieval (sweet spot for speed/accuracy)
        // Use temperature = 0.2 for highly deterministic, accurate answers
        var kbResponse = await _knowledgeBaseService.RetrieveAndGenerateAsync(
            processedQuestion,
            modelId: null,
            maxResults: 50,
            temperature: 0.2,
            instruction: finalInstruction
        );

        if (!kbResponse.Success || string.IsNullOrEmpty(kbResponse.Answer))
        {
            var errorMsg = kbResponse.Error ?? "Unknown error";
            _logger.LogWarning("Failed to get answer from Knowledge Base: {Error}", errorMsg);
            return null;
        }

        var answer = kbResponse.Answer.Trim();

        // RETRY DISABLED FOR SPEED - Uncomment if you need higher accuracy at cost of speed
        /*
        // Check if answer indicates uncertainty or not found
        if (answer.Contains("ขออภัย", StringComparison.Ordinal) || 
            answer.Contains("ไม่พบข้อมูล", StringComparison.Ordinal) ||
            answer.Contains("ไม่ทราบ", StringComparison.Ordinal))
        {
            _logger.LogInformation("Answer indicates uncertainty, retrying with different strategy");

            var retryResponse = await _knowledgeBaseService.RetrieveAndGenerateAsync(
                processedQuestion,
                modelId: null,
                maxResults: 30,
                temperature: 0.5,
                instruction: finalInstruction
            );

            if (retryResponse.Success && !string.IsNullOrEmpty(retryResponse.Answer))
            {
                var retryAnswer = retryResponse.Answer.Trim();
                
                if (!retryAnswer.Contains("ขออภัย", StringComparison.Ordinal) &&
                    !retryAnswer.Contains("ไม่พบข้อมูล", StringComparison.Ordinal))
                {
                    answer = retryAnswer;
                    _logger.LogInformation("Retry successful with better answer");
                }
            }
        }
        */

        return answer;
    }

    private string PreprocessQuery(string query)
    {
        // Minimal preprocessing for speed
        query = query.Trim();
        
        // Only remove excessive whitespace (fast operation)
        if (query.Contains("  "))
        {
            query = System.Text.RegularExpressions.Regex.Replace(query, @"\s+", " ");
        }
        
        return query;
    }
}
