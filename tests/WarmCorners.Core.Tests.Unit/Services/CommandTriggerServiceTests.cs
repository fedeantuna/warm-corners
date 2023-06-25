using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using WarmCorners.Core.Common;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Core.Wrappers;

namespace WarmCorners.Core.Tests.Unit.Services;

public class CommandTriggerServiceTests
{
    private readonly ICommandTriggerService _commandTriggerService;
    private readonly Mock<IProcessWrapper> _processWrapperMock;
    private readonly Mock<IScreenService> _screenServiceMock;

    public CommandTriggerServiceTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        var screenService = provider.GetRequiredService<IScreenService>();
        this._screenServiceMock = Mock.Get(screenService);
        var processWrapper = provider.GetRequiredService<IProcessWrapper>();
        this._processWrapperMock = Mock.Get(processWrapper);

        this._commandTriggerService = provider.GetRequiredService<ICommandTriggerService>();
    }

    [Fact]
    public void ProcessCommandTrigger_LogsWhenACommandHasBeenExecutedCorrectly()
    {
        // Arrange
        const string command = "run some-program 1";
        const int topLeftCornerX = 0;
        const int topLeftCornerY = 0;
        var commandTriggers = new List<CommandTrigger>
        {
            new()
            {
                ScreenCorner = ScreenCorner.TopLeft,
                Command = command
            }
        };

        this._screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(ScreenCorner.TopLeft, topLeftCornerX, topLeftCornerY))
            .Returns(true);
        this._processWrapperMock
            .Setup(pw => pw.Start("cmd", $"/c {command}"))
            .Returns(true);

        // Act
        this._commandTriggerService.ProcessCommandTrigger(commandTriggers, topLeftCornerX, topLeftCornerY);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage("Executed {Command}").Once()
            .WithLevel(LogEventLevel.Information)
            .WithProperty("Command").WithValue(command);
    }

    [Fact]
    public void ProcessCommandTrigger_LogsWhenACommandHasBeenExecutedIncorrectly()
    {
        // Arrange
        const string command = "run some-program 1";
        const int topRightCornerX = 768;
        const int topRightCornerY = 0;
        var commandTriggers = new List<CommandTrigger>
        {
            new()
            {
                ScreenCorner = ScreenCorner.TopRight,
                Command = command
            }
        };

        this._screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(ScreenCorner.TopRight, topRightCornerX, topRightCornerY))
            .Returns(true);
        this._processWrapperMock
            .Setup(pw => pw.Start("cmd", $"/c {command}"))
            .Returns(false);

        // Act
        this._commandTriggerService.ProcessCommandTrigger(commandTriggers, topRightCornerX, topRightCornerY);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage("Could not run {Command}").Once()
            .WithLevel(LogEventLevel.Error)
            .WithProperty("Command").WithValue(command);
    }

    [Fact]
    public void ProcessCommandTrigger_ExecutesCommandsWhenMouseCursorIsInCorrectCorner()
    {
        // Arrange
        const string bottomRightCornerCommand = "run some-program 1";
        const string bottomLeftCornerCommand = "run some-program 2";
        const int bottomLeftCornerX = 0;
        const int bottomLeftCornerY = 1024;
        var commandTriggers = new List<CommandTrigger>
        {
            new()
            {
                ScreenCorner = ScreenCorner.BottomRight,
                Command = bottomRightCornerCommand
            },
            new()
            {
                ScreenCorner = ScreenCorner.BottomLeft,
                Command = bottomLeftCornerCommand
            }
        };

        this._screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(ScreenCorner.BottomRight, bottomLeftCornerX, bottomLeftCornerY))
            .Returns(false);
        this._screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(ScreenCorner.BottomLeft, bottomLeftCornerX, bottomLeftCornerY))
            .Returns(true);

        // Act
        this._commandTriggerService.ProcessCommandTrigger(commandTriggers, bottomLeftCornerX, bottomLeftCornerY);

        // Assert
        this._processWrapperMock.Verify(pw => pw.Start("cmd", $"/c {bottomRightCornerCommand}"), Times.Never);
        this._processWrapperMock.Verify(pw => pw.Start("cmd", $"/c {bottomLeftCornerCommand}"), Times.Once);
    }
}
