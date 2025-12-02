namespace AbdulBackend.Models;

public class HackathonResponse
{
    public string Uuid { get; set; } = string.Empty;
    public string PassKeyType { get; set; } = string.Empty;
    public int MaxDurationInSecs { get; set; }
    public decimal TimeUsedInSeconds { get; set; }
    public int TotalQuestion { get; set; }
    public decimal MaximumScore { get; set; }
    public int AnsweredQuestion { get; set; }
    public decimal Score { get; set; }
    public List<QuestionResult> Results { get; set; } = new();
}