namespace AbdulBackend.Models;

public class QuestionResult
{
    public int No { get; set; }
    public string Question { get; set; } = string.Empty;
    public string ExpectedAnswer { get; set; } = string.Empty;
    public string? ActualAnswer { get; set; }
    public decimal Score { get; set; }
}