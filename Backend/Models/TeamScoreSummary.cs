namespace AbdulBackend.Models;

public class TeamScoreSummary
{
    public string Team { get; set; } = string.Empty;
    public decimal TotalScore { get; set; }
    public decimal TimeUsedInSeconds { get; set; }
    public int AnsweredQuestion { get; set; }
    public int TotalQuestion { get; set; }
    public decimal MaximumScore { get; set; }
}