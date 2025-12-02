using AbdulBackend.Models;

namespace AbdulBackend.Services;

public interface IAnnouncementService
{
    Task<List<Announcement>> GetLatestAnnouncementsAsync();
}