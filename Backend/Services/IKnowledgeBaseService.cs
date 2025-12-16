using AbdulBackend.Models;

namespace AbdulBackend.Services;

public interface IKnowledgeBaseService
{
    Task<RetrieveOnlyResponse> RetrieveAsync(string query, int maxResults = 5);
    Task<KnowledgeBaseResponse> RetrieveAndGenerateAsync(string query, string? modelId = null, int maxResults = 5);
    Task<KnowledgeBaseResponse> RetrieveAndGenerateAsync(string query, string? modelId = null, int maxResults = 5, double? temperature = null);
    Task<KnowledgeBaseResponse> RetrieveAndGenerateAsync(string query, string? modelId = null, int maxResults = 5, double? temperature = null, string? instruction = null);
}
