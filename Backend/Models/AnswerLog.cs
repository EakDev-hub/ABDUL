namespace AbdulBackend.Models;

public class AnswerLog
{
    public string Uuid { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public string PassKey { get; set; } = string.Empty;
    public string PassKeyType { get; set; } = string.Empty;
    public int MaxDurationInSeconds { get; set; }
    public int TotalQuestion { get; set; }
    public int AnsweredQuestion { get; set; }
    public decimal MaximumScore { get; set; }
    public decimal TimeUsedInSeconds { get; set; }
    public decimal Score { get; set; }
    public DateTime CreatedAt { get; set; }
}