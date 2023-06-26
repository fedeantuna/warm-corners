using FluentAssertions;
using WarmCorners.Core.Common;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Service.Configurations;
using WarmCorners.Service.Mappers;

namespace WarmCorners.Service.Tests.Unit.Mappers;

public class CommandTriggerMapperTests
{
    [Fact]
    public void ToCommandTriggers_MapsConfigurationToCommandTriggersIgnoringCase()
    {
        // Arrange
        var commandTriggerConfigurations = new List<CommandTriggerConfiguration>
        {
            new()
            {
                ScreenCorner = "topLEFT",
                Command = Testing.SomeCommand
            },
            new()
            {
                ScreenCorner = "topRight",
                Command = Testing.SomeOtherCommand
            }
        };

        // Act
        var result = commandTriggerConfigurations.ToCommandTriggers().ToList();

        // Assert
        result.Should().HaveCount(commandTriggerConfigurations.Count);
        VerifyCommandOnTopLeftScreenCorner(result);
        VerifyCommandOnTopRightScreenCorner(result);
    }

    [Fact]
    public void ToCommandTriggers_MapsConfigurationToCommandTriggersIgnoringInvalidScreenCorners()
    {
        // Arrange
        var commandTriggerConfigurations = new List<CommandTriggerConfiguration>
        {
            new()
            {
                ScreenCorner = "centerCenter",
                Command = Testing.SomeCommand
            },
            new()
            {
                ScreenCorner = "TopRIGHT",
                Command = Testing.SomeOtherCommand
            }
        };

        // Act
        var result = commandTriggerConfigurations.ToCommandTriggers().ToList();

        // Assert
        result.Should().HaveCount(1);
        VerifyCommandOnTopRightScreenCorner(result);
    }

    private static void VerifyCommandOnTopLeftScreenCorner(IEnumerable<CommandTrigger> result) =>
        result.Should().ContainSingle(ct =>
            ct.ScreenCorner == ScreenCorner.TopLeft && ct.Command == Testing.SomeCommand);

    private static void VerifyCommandOnTopRightScreenCorner(IEnumerable<CommandTrigger> result) =>
        result.Should().ContainSingle(ct =>
            ct.ScreenCorner == ScreenCorner.TopRight && ct.Command == Testing.SomeOtherCommand);
}
