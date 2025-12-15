namespace AbdulBackend.Services;

public interface IBedrockService
{
    Task<string> InvokeModelAsync(string prompt, string? modelId = null);
    Task<string> InvokeModelWithSystemPromptAsync(string systemPrompt, string userPrompt, string? modelId = null);
}
