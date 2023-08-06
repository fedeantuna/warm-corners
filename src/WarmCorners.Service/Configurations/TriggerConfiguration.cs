using System.Diagnostics.CodeAnalysis;

namespace WarmCorners.Service.Configurations;

public class TriggerConfiguration
{
    public List<ShellTriggerConfiguration> ShellTriggers { get; init; } = new();
    public List<KeyCombinationTriggerConfiguration> KeyCombinationTriggers { get; init; } = new();
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ShellTriggerConfiguration
{
    public required string ScreenCorner { get; init; }
    public required string ShellCommand { get; init; }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class KeyCombinationTriggerConfiguration
{
    public required string ScreenCorner { get; init; }
    public required string KeyCombination { get; init; }
}
