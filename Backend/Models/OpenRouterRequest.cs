using System.Text.Json.Serialization;

namespace AbdulBackend.Models;

public class OpenRouterRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("messages")]
    public List<OpenRouterMessage> Messages { get; set; } = new();
    
    [JsonPropertyName("temperature")]
    public decimal Temperature { get; set; } = 0;
}

public class OpenRouterMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
    
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}