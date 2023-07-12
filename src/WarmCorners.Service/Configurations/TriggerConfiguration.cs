namespace WarmCorners.Service.Configurations;

public class TriggerConfiguration
{
    public List<ShellTriggerConfiguration> CommandTriggers { get; init; } = new();
    public List<KeyCombinationTriggerConfiguration> KeyCombinationTriggers { get; init; } = new();
}

public class ShellTriggerConfiguration
{
    public required string ScreenCorner { get; init; }
    public required string ShellCommand { get; init; }
}

public class KeyCombinationTriggerConfiguration
{
    public required string ScreenCorner { get; init; }
    public required string KeyCombination { get; init; }
}
