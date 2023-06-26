namespace WarmCorners.Service.Configurations;

public class TriggerConfiguration
{
    public List<CommandTriggerConfiguration> CommandTriggers { get; init; } = new();
    public List<KeyCombinationTriggerConfiguration> KeyCombinationTriggers { get; init; } = new();
}

public class CommandTriggerConfiguration
{
    public required string ScreenCorner { get; init; }
    public required string Command { get; init; }
}

public class KeyCombinationTriggerConfiguration
{
    public required string ScreenCorner { get; init; }
    public required string KeyCombination { get; init; }
}
