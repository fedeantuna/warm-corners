using WarmCorners.Core.Common;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Service.Configurations;

namespace WarmCorners.Service.Mappers;

public static class CommandTriggerMapper
{
    public static IEnumerable<CommandTrigger> ToCommandTriggers(this List<CommandTriggerConfiguration> commandTriggerConfigurations)
    {
        foreach (var commandTriggerConfiguration in commandTriggerConfigurations)
            if (Enum.TryParse(typeof(ScreenCorner), commandTriggerConfiguration.ScreenCorner, true, out var screenCorner))
                yield return new CommandTrigger
                {
                    ScreenCorner = (ScreenCorner)screenCorner,
                    Command = commandTriggerConfiguration.Command
                };
    }
}
