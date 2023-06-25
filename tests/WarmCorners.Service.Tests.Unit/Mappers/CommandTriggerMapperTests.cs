using FluentAssertions;
using WarmCorners.Core.Common;
using WarmCorners.Service.Configurations;
using WarmCorners.Service.Mappers;

namespace WarmCorners.Service.Tests.Unit.Mappers;

public class CommandTriggerMapperTests
{
    [Fact]
    public void ToCommandTriggers_MapsConfigurationToCommandTriggersIgnoringCase()
    {
        // Arrange
        const string topLeftCornerCommand = "some command";
        const string topRightCornerCommand = "some other command";
        var commandTriggerConfigurations = new List<CommandTriggerConfiguration>
        {
            new()
            {
                ScreenCorner = "topLEFT",
                Command = topLeftCornerCommand
            },
            new()
            {
                ScreenCorner = "topRight",
                Command = topRightCornerCommand
            }
        };

        // Act
        var result = commandTriggerConfigurations.ToCommandTriggers().ToList();

        // Assert
        result.Should().HaveCount(commandTriggerConfigurations.Count);
        result.Should().Contain(ct => ct.ScreenCorner == ScreenCorner.TopLeft && ct.Command == topLeftCornerCommand);
        result.Should().Contain(ct => ct.ScreenCorner == ScreenCorner.TopRight && ct.Command == topRightCornerCommand);
    }

    [Fact]
    public void ToCommandTriggers_MapsConfigurationToCommandTriggersIgnoringInvalidScreenCorners()
    {
        // Arrange
        const string centerCenterCornerCommand = "some other command";
        const string topRightCornerCommand = "some command";
        var commandTriggerConfigurations = new List<CommandTriggerConfiguration>
        {
            new()
            {
                ScreenCorner = "centerCenter",
                Command = centerCenterCornerCommand
            },
            new()
            {
                ScreenCorner = "topRight",
                Command = topRightCornerCommand
            }
        };

        // Act
        var result = commandTriggerConfigurations.ToCommandTriggers().ToList();

        // Assert
        result.Should().ContainSingle(ct => ct.ScreenCorner == ScreenCorner.TopRight && ct.Command == topRightCornerCommand);
    }
}
