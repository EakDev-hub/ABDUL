namespace AbdulBackend.Models;

public class UpdateAnnouncementDto
{
    public string? Text { get; set; }
    public DateTime? PostedAt { get; set; }
    public bool? IsActive { get; set; }
}