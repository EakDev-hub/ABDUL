namespace AbdulBackend.Models;

public class BedrockRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string? SystemPrompt { get; set; }
    public string? ModelId { get; set; }
}

public class BedrockResponse
{
    public bool Success { get; set; }
    public string? Response { get; set; }
    public string? Error { get; set; }
    public string? ModelId { get; set; }
}
