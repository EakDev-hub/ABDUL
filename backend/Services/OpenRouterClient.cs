using AbdulBackend.Models;
using System.Net.Http.Json;

namespace AbdulBackend.Services;

public class OpenRouterClient : IOpenRouterClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;
    private readonly ILogger<OpenRouterClient> _logger;

    public OpenRouterClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<OpenRouterClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = configuration["OPENROUTER_API_KEY"] 
            ?? throw new InvalidOperationException("OPENROUTER_API_KEY is not configured in environment variables");
        _logger = logger;
    }

    public async Task<decimal> EvaluateAnswerAsync(
        string instruction,
        string model,
        string question,
        string expectedAnswer,
        string actualAnswer)
    {
        // Don't evaluate errors or timeouts
        if (string.IsNullOrEmpty(actualAnswer) || 
            actualAnswer.Contains("timeout", StringComparison.OrdinalIgnoreCase) || 
            actualAnswer.Contains("error", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Skipping AI evaluation for error/timeout answer");
            return 0m;
        }

        try
        {
            return await CallOpenRouterApiAsync(instruction, model, question, expectedAnswer, actualAnswer);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenRouter API failed, retrying once...");
            
            // Retry once
            try
            {
                await Task.Delay(1000); // Brief delay before retry
                return await CallOpenRouterApiAsync(instruction, model, question, expectedAnswer, actualAnswer);
            }
            catch (Exception retryEx)
            {
                _logger.LogError(retryEx, "OpenRouter API failed after retry, returning score = 0");
                return 0m;
            }
        }
    }

    private async Task<decimal> CallOpenRouterApiAsync(
        string instruction,
        string model,
        string question,
        string expectedAnswer,
        string actualAnswer)
    {
        // Replace placeholders in instruction
        var prompt = instruction
            .Replace("{QUESTION}", question)
            .Replace("{EXPECTED_ANSWER}", expectedAnswer)
            .Replace("{ACTUAL_ANSWER}", actualAnswer);

        var client = _httpClientFactory.CreateClient("OpenRouter");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var request = new OpenRouterRequest
        {
            Model = model,
            Messages = new List<OpenRouterMessage>
            {
                new() { Role = "user", Content = prompt }
            }
        };

        _logger.LogInformation("Calling OpenRouter API with model: {Model}", model);

        var response = await client.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("OpenRouter API error ({StatusCode}): {Error}", response.StatusCode, errorContent);
            throw new HttpRequestException($"OpenRouter API returned {response.StatusCode}: {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<OpenRouterResponse>();
        var aiResponse = result?.Choices?.FirstOrDefault()?.Message?.Content ?? "";

        if (string.IsNullOrEmpty(aiResponse))
        {
            _logger.LogWarning("OpenRouter returned empty response");
            return 0m;
        }

        var score = ParseScoreFromResponse(aiResponse);
        _logger.LogInformation("AI evaluation score: {Score}", score);
        
        return score;
    }

    private decimal ParseScoreFromResponse(string aiResponse)
    {
        try
        {
            // Expected format: "Score: 0.8\nReasoning: ..."
            var lines = aiResponse.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var scoreLine = lines.FirstOrDefault(l => 
                l.StartsWith("Score:", StringComparison.OrdinalIgnoreCase));

            if (scoreLine != null)
            {
                var scoreText = scoreLine.Replace("Score:", "", StringComparison.OrdinalIgnoreCase).Trim();
                if (decimal.TryParse(scoreText, out var score))
                {
                    // Clamp score between 0 and 1
                    return Math.Clamp(score, 0m, 1m);
                }
            }

            _logger.LogWarning("Could not parse score from AI response: {Response}", aiResponse);
            return 0m;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing score from AI response");
            return 0m;
        }
    }
}