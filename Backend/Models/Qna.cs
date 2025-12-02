namespace AbdulBackend.Models;

public class Qna
{
    public long Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}