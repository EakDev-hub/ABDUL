namespace AbdulBackend.Models;

public class AccuracyTestRequest
{
    public string Question { get; set; } = string.Empty;
    public string? Instruction { get; set; }
}

public class AccuracyTestResponse
{
    public string Answer { get; set; } = string.Empty;
    public bool IsSuggestion { get; set; } = false;
    public double Temperature { get; set; } = 0.2;
}

public class AccuracyTestBatchRequest
{
    public List<string> Questions { get; set; } = new();
    public string? Instruction { get; set; }
}

public class AccuracyTestBatchResult
{
    public int Index { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public bool Success { get; set; }
    public bool IsSuggestion { get; set; } = false;
    public double Temperature { get; set; } = 0.2;
}

public class AccuracyTestBatchResponse
{
    public List<AccuracyTestBatchResult> Results { get; set; } = new();
    public int TotalQuestions { get; set; }
    public int SuccessCount { get; set; }
}
