namespace AbdulBackend.Models;

public class KnowledgeBaseRequest
{
    public string Query { get; set; } = string.Empty;
    public int MaxResults { get; set; } = 3;
    public string? ModelId { get; set; }
}

public class KnowledgeBaseResponse
{
    public bool Success { get; set; }
    public string? Answer { get; set; }
    public List<RetrievalResult>? Results { get; set; }
    public List<KnowledgeBaseCitation>? Citations { get; set; }
    public string? Error { get; set; }
    public string? SessionId { get; set; }
}

public class RetrievalResult
{
    public string Content { get; set; } = string.Empty;
    public string? DocumentTitle { get; set; }
    public string? DocumentUri { get; set; }
    public double Score { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class KnowledgeBaseCitation
{
    public string? Text { get; set; }
    public List<RetrievalResult>? RetrievedReferences { get; set; }
}

public class RetrieveOnlyRequest
{
    public string Query { get; set; } = string.Empty;
    public int MaxResults { get; set; } = 3;
}

public class RetrieveOnlyResponse
{
    public bool Success { get; set; }
    public List<RetrievalResult>? Results { get; set; }
    public string? Error { get; set; }
}
