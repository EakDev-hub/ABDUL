using AbdulBackend.Models;

namespace AbdulBackend.Services;

public interface IQnaService
{
    Task<List<Qna>> GetLatestQnaAsync();
    Task<List<Qna>> GetAllQnaAsync();
    Task<Qna?> GetQnaByIdAsync(long id);
    Task<Qna> CreateQnaAsync(string question, string? answer);
    Task<Qna?> UpdateQnaAsync(long id, string? question, string? answer);
    Task<bool> DeleteQnaAsync(long id);
}