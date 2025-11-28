using System.Text.Json.Serialization;

namespace AbdulBackend.Models;

public class OpenRouterResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("choices")]
    public List<OpenRouterChoice> Choices { get; set; } = new();
}

public class OpenRouterChoice
{
    [JsonPropertyName("message")]
    public OpenRouterMessage Message { get; set; } = new();
}