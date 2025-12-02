namespace AbdulBackend.Models;

public class AiEvaluationInstruction
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Instruction { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}