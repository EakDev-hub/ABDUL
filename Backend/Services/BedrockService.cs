using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using System.Text;
using System.Text.Json;

namespace AbdulBackend.Services;

public class BedrockService : IBedrockService
{
    private readonly AmazonBedrockRuntimeClient _bedrockClient;
    private readonly string _defaultModelId;
    private readonly ILogger<BedrockService> _logger;

    public BedrockService(IConfiguration configuration, ILogger<BedrockService> logger)
    {
        _logger = logger;

        var accessKey = configuration["AWS_ACCESS_KEY_ID"] 
            ?? throw new InvalidOperationException("AWS_ACCESS_KEY_ID is not configured");
        var secretKey = configuration["AWS_SECRET_ACCESS_KEY"] 
            ?? throw new InvalidOperationException("AWS_SECRET_ACCESS_KEY is not configured");
        var sessionToken = configuration["AWS_SESSION_TOKEN"];
        var region = configuration["AWS_REGION"] ?? configuration["AWS_DEFAULT_REGION"] ?? "us-east-1";
        _defaultModelId = configuration["AWS_BEDROCK_MODEL_ID"] ?? "us.anthropic.claude-haiku-4-5-v1:0";

        var regionEndpoint = RegionEndpoint.GetBySystemName(region);
        
        // Support both permanent and temporary credentials
        if (!string.IsNullOrEmpty(sessionToken))
        {
            var credentials = new Amazon.Runtime.SessionAWSCredentials(accessKey, secretKey, sessionToken);
            _bedrockClient = new AmazonBedrockRuntimeClient(credentials, regionEndpoint);
            _logger.LogInformation("Bedrock service initialized with temporary credentials (session token)");
        }
        else
        {
            _bedrockClient = new AmazonBedrockRuntimeClient(accessKey, secretKey, regionEndpoint);
            _logger.LogInformation("Bedrock service initialized with permanent credentials");
        }

        _logger.LogInformation("Region: {Region}, Model: {Model}", region, _defaultModelId);
    }

    public async Task<string> InvokeModelAsync(string prompt, string? modelId = null)
    {
        return await InvokeModelWithSystemPromptAsync(string.Empty, prompt, modelId);
    }

    public async Task<string> InvokeModelWithSystemPromptAsync(string systemPrompt, string userPrompt, string? modelId = null)
    {
        var model = modelId ?? _defaultModelId;

        try
        {
            // Build request payload for Claude models
            var requestBody = new
            {
                anthropic_version = "bedrock-2023-05-31",
                max_tokens = 4096,
                temperature = 0.7,
                system = string.IsNullOrEmpty(systemPrompt) ? null : systemPrompt,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = userPrompt
                    }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            _logger.LogDebug("Invoking Bedrock model: {Model}", model);

            var request = new InvokeModelRequest
            {
                ModelId = model,
                Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonPayload)),
                ContentType = "application/json",
                Accept = "application/json"
            };

            var response = await _bedrockClient.InvokeModelAsync(request);

            using var reader = new StreamReader(response.Body);
            var responseBody = await reader.ReadToEndAsync();

            _logger.LogDebug("Bedrock response received");

            // Parse Claude response
            var jsonResponse = JsonDocument.Parse(responseBody);
            var content = jsonResponse.RootElement
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString();

            return content ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invoking Bedrock model: {Model}", model);
            throw;
        }
    }
}
