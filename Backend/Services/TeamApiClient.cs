using AbdulBackend.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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
            var request = new TeamApiRequest { Question = question };

            _logger.LogInformation("Calling team API: {ApiUrl}", apiUrl);

            // Parse URL and extract host for Host header
            var uri = new Uri(apiUrl);
            var host = uri.IsDefaultPort ? uri.Host : $"{uri.Host}:{uri.Port}";

            // Serialize request to JSON
            var jsonContent = JsonSerializer.Serialize(request);
            
            // Create custom HttpClientHandler with specific settings
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };
            
            using var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(60);
            client.DefaultRequestHeaders.Clear();
            
            // Create HTTP request
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Version = new Version(1, 1), // Force HTTP/1.1
                Content = new StringContent(jsonContent, Encoding.UTF8)
            };
            
            // Set headers explicitly
            httpRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpRequest.Headers.Host = host;
            httpRequest.Headers.ConnectionClose = false;

            _logger.LogInformation("Sending request - Host: {Host}, Content-Type: application/json, Content-Length: {Length}",
                host, httpRequest.Content.Headers.ContentLength);

            var response = await client.SendAsync(httpRequest, cancellationToken);
            
            _logger.LogInformation("Received response - StatusCode: {StatusCode}", response.StatusCode);
            
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