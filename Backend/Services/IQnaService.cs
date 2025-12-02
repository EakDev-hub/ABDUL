using AbdulBackend.Models;

namespace AbdulBackend.Services;

public interface IQnaService
{
    Task<List<Qna>> GetLatestQnaAsync();
}