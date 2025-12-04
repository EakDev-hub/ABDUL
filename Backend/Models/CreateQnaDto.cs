namespace AbdulBackend.Models;

public class CreateQnaDto
{
    public string Question { get; set; } = string.Empty;
    public string? Answer { get; set; }
}