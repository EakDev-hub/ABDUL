namespace AbdulBackend.Models;

public class TeamPassKey
{
    public long Id { get; set; }
    public string PassKey { get; set; } = string.Empty;
    public string PassKeyType { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public int MaxDurationInSeconds { get; set; }
}