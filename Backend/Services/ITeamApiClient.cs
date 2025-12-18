namespace AbdulBackend.Services;

public interface ITeamApiClient
{
    Task<string> AskQuestionAsync(string apiUrl, string question, CancellationToken cancellationToken, string teamName);
}