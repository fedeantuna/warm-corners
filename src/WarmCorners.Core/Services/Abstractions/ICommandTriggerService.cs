using WarmCorners.Core.Common;

namespace WarmCorners.Core.Services.Abstractions;

public interface ICommandTriggerService
{
    void ProcessCommandTrigger(IEnumerable<CommandTrigger> commandTrigger, int x, int y);
}

public class CommandTrigger
{
    public required ScreenCorner ScreenCorner { get; init; }
    public required string Command { get; init; }
}
