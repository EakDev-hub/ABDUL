using AbdulBackend.Models;
using System.Net.Http.Json;

namespace AbdulBackend.Services;

public class TeamApiClient : ITeamApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TeamApiClient> _logger;

    public TeamApiClient(IHttpClientFactory httpClientFactory, ILogger<TeamApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string> AskQuestionAsync(string apiUrl, string question, CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("TeamApi");
            var request = new TeamApiRequest { Question = question };

            _logger.LogInformation("Calling team API: {ApiUrl}", apiUrl);

            var response = await client.PostAsJsonAsync(apiUrl, request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TeamApiResponse>(cancellationToken);
            
            if (result == null || string.IsNullOrEmpty(result.Answer))
            {
                _logger.LogWarning("Team API returned empty or null answer");
                return "{\"error\": \"Empty response from team API\"}";
            }

            _logger.LogInformation("Team API responded successfully");
            return result.Answer;
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Team API request timeout for URL: {ApiUrl}", apiUrl);
            return "Request timeout";
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Team API HTTP error for URL: {ApiUrl}", apiUrl);
            return $"{{\"error\": \"{ex.Message}\"}}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Team API unexpected error for URL: {ApiUrl}", apiUrl);
            return $"{{\"error\": \"{ex.Message}\"}}";
        }
    }
}