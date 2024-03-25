namespace Core;

public class InteractionPoint
{
    public int Key { get; set; }
    public ProcessState State { get; set; } = ProcessState.Received;

    public string Content { get; set; } = string.Empty;
}