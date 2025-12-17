using AbdulBackend.Models;

namespace AbdulBackend.Services;

public interface IDisplayControlService
{
    Task<DisplayControl?> GetDisplayControlAsync();
    Task<DisplayControl?> UpdateDisplayControlAsync(UpdateDisplayControlDto dto);
}
