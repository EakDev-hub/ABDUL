namespace AbdulBackend.Services;

public interface IOpenRouterClient
{
    Task<decimal> EvaluateAnswerAsync(
        string instruction,
        string model,
        string question,
        string expectedAnswer,
        string actualAnswer
    );
}