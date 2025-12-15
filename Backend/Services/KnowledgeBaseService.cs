using Amazon;
using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using AbdulBackend.Models;

namespace AbdulBackend.Services;

public class KnowledgeBaseService : IKnowledgeBaseService
{
    private readonly AmazonBedrockAgentRuntimeClient _agentClient;
    private readonly string _knowledgeBaseId;
    private readonly string _defaultModelId;
    private readonly ILogger<KnowledgeBaseService> _logger;

    public KnowledgeBaseService(IConfiguration configuration, ILogger<KnowledgeBaseService> logger)
    {
        _logger = logger;

        var accessKey = configuration["AWS_ACCESS_KEY_ID"]
            ?? throw new InvalidOperationException("AWS_ACCESS_KEY_ID is not configured");
        var secretKey = configuration["AWS_SECRET_ACCESS_KEY"]
            ?? throw new InvalidOperationException("AWS_SECRET_ACCESS_KEY is not configured");
        var sessionToken = configuration["AWS_SESSION_TOKEN"];
        var region = configuration["AWS_REGION"] ?? configuration["AWS_DEFAULT_REGION"] ?? "us-east-1";
        
        _knowledgeBaseId = configuration["AWS_KNOWLEDGE_BASE_ID"]
            ?? throw new InvalidOperationException("AWS_KNOWLEDGE_BASE_ID is not configured");
        _defaultModelId = configuration["AWS_BEDROCK_MODEL_ID"] ?? "us.anthropic.claude-haiku-4-5-v1:0";

        var regionEndpoint = RegionEndpoint.GetBySystemName(region);

        // Support both permanent and temporary credentials
        if (!string.IsNullOrEmpty(sessionToken))
        {
            var credentials = new Amazon.Runtime.SessionAWSCredentials(accessKey, secretKey, sessionToken);
            _agentClient = new AmazonBedrockAgentRuntimeClient(credentials, regionEndpoint);
            _logger.LogInformation("Knowledge Base service initialized with temporary credentials");
        }
        else
        {
            _agentClient = new AmazonBedrockAgentRuntimeClient(accessKey, secretKey, regionEndpoint);
            _logger.LogInformation("Knowledge Base service initialized with permanent credentials");
        }

        _logger.LogInformation("Knowledge Base ID: {KnowledgeBaseId}, Region: {Region}", _knowledgeBaseId, region);
    }

    private string BuildModelArn(string modelId)
    {
        // For cross-region inference profiles (us.anthropic.*, eu.anthropic.*)
        if (modelId.StartsWith("us.") || modelId.StartsWith("eu."))
        {
            // Sonnet 4.5 uses inference profile ID directly (no ARN prefix)
            return modelId;
        }
        
        // For standard foundation models (anthropic.claude-3-*)
        return $"arn:aws:bedrock:us-east-1::foundation-model/{modelId}";
    }

    public async Task<RetrieveOnlyResponse> RetrieveAsync(string query, int maxResults = 5)
    {
        try
        {
            _logger.LogDebug("Retrieving documents for query: {Query}", query);

            var request = new RetrieveRequest
            {
                KnowledgeBaseId = _knowledgeBaseId,
                RetrievalQuery = new KnowledgeBaseQuery
                {
                    Text = query
                },
                RetrievalConfiguration = new KnowledgeBaseRetrievalConfiguration
                {
                    VectorSearchConfiguration = new KnowledgeBaseVectorSearchConfiguration
                    {
                        NumberOfResults = maxResults
                    }
                }
            };

            var response = await _agentClient.RetrieveAsync(request);

            var results = response.RetrievalResults.Select(r => new RetrievalResult
            {
                Content = r.Content?.Text ?? string.Empty,
                Score = r.Score,
                DocumentUri = r.Location?.S3Location?.Uri,
                Metadata = r.Metadata?.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (object)(kvp.Value.ToString() ?? string.Empty)
                )
            }).ToList();

            _logger.LogInformation("Retrieved {Count} documents", results.Count);

            return new RetrieveOnlyResponse
            {
                Success = true,
                Results = results
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving from Knowledge Base");
            return new RetrieveOnlyResponse
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public async Task<KnowledgeBaseResponse> RetrieveAndGenerateAsync(string query, string? modelId = null, int maxResults = 5)
    {
        try
        {
            _logger.LogDebug("Retrieving and generating answer for query: {Query}", query);

            var model = modelId ?? _defaultModelId;

            var request = new RetrieveAndGenerateRequest
            {
                Input = new RetrieveAndGenerateInput
                {
                    Text = query
                },
                RetrieveAndGenerateConfiguration = new RetrieveAndGenerateConfiguration
                {
                    Type = RetrieveAndGenerateType.KNOWLEDGE_BASE,
                    KnowledgeBaseConfiguration = new KnowledgeBaseRetrieveAndGenerateConfiguration
                    {
                        KnowledgeBaseId = _knowledgeBaseId,
                        ModelArn = BuildModelArn(model),
                        RetrievalConfiguration = new KnowledgeBaseRetrievalConfiguration
                        {
                            VectorSearchConfiguration = new KnowledgeBaseVectorSearchConfiguration
                            {
                                NumberOfResults = maxResults
                            }
                        }
                    }
                }
            };

            var response = await _agentClient.RetrieveAndGenerateAsync(request);

            var answer = response.Output?.Text ?? string.Empty;
            var sessionId = response.SessionId;

            // Parse citations
            var citations = response.Citations?.Select(c => new KnowledgeBaseCitation
            {
                Text = c.GeneratedResponsePart?.TextResponsePart?.Text,
                RetrievedReferences = c.RetrievedReferences?.Select(r => new RetrievalResult
                {
                    Content = r.Content?.Text ?? string.Empty,
                    DocumentUri = r.Location?.S3Location?.Uri,
                    Metadata = r.Metadata?.ToDictionary(
                        kvp => kvp.Key,
                        kvp => (object)(kvp.Value.ToString() ?? string.Empty)
                    )
                }).ToList()
            }).ToList();

            _logger.LogInformation("Generated answer with {CitationCount} citations", citations?.Count ?? 0);

            return new KnowledgeBaseResponse
            {
                Success = true,
                Answer = answer,
                Citations = citations,
                SessionId = sessionId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RetrieveAndGenerate");
            return new KnowledgeBaseResponse
            {
                Success = false,
                Error = ex.Message
            };
        }
    }
}
