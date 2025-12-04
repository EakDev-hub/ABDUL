using AbdulBackend.Models;

namespace AbdulBackend.Services;

public interface IAnnouncementService
{
    Task<List<Announcement>> GetLatestAnnouncementsAsync();
    Task<List<Announcement>> GetAllAnnouncementsAsync();
    Task<Announcement?> GetAnnouncementByIdAsync(long id);
    Task<Announcement> CreateAnnouncementAsync(string text, DateTime postedAt, bool isActive);
    Task<Announcement?> UpdateAnnouncementAsync(long id, string? text, DateTime? postedAt, bool? isActive);
    Task<bool> DeleteAnnouncementAsync(long id);
}