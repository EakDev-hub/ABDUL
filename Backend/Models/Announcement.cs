namespace AbdulBackend.Models;

public class Announcement
{
    public long Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime PostedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}