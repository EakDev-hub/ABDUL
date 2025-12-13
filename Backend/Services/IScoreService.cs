using AbdulBackend.Models;

namespace AbdulBackend.Services;

public interface IScoreService
{
    Task<List<TeamScoreSummary>> GetTeamScoreSummaryAsync(string? passKeyType = null);
}