namespace AbdulBackend.Models;

public class CreateAnnouncementDto
{
    public string Text { get; set; } = string.Empty;
    public DateTime? PostedAt { get; set; }
    public bool IsActive { get; set; } = true;
}