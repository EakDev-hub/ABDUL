namespace AbdulBackend.Models;

public class Question
{
    public long Id { get; set; }
    public string PassKeyType { get; set; } = string.Empty;
    public int No { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string ExpectedAnswer { get; set; } = string.Empty;
}