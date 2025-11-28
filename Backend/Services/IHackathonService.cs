using AbdulBackend.Models;

namespace AbdulBackend.Services;

public interface IHackathonService
{
    Task<HackathonResponse> ProcessHackathonAsync(HackathonRequest request);
}