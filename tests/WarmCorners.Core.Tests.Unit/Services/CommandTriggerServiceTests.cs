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

    public CommandTriggerServiceTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        var processWrapper = provider.GetRequiredService<IProcessWrapper>();
        this._processWrapperMock = Mock.Get(processWrapper);

        this._commandTriggerService = provider.GetRequiredService<ICommandTriggerService>();
    }

    [Fact]
    public void ProcessCommandTrigger_LogsWhenACommandHasBeenExecutedCorrectly()
    {
        // Arrange
        var commandTriggers = new List<CommandTrigger>
        {
            new()
            {
                ScreenCorner = ScreenCorner.TopLeft,
                Command = Testing.CommandThatRunsSuccessfully
            }
        };
        
        // Act
        this._commandTriggerService.ProcessCommandTrigger(commandTriggers, Testing.TopLeftCorner.X, Testing.TopLeftCorner.Y);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage("Executed {Command}").Once()
            .WithLevel(LogEventLevel.Information)
            .WithProperty("Command").WithValue(Testing.CommandThatRunsSuccessfully);
    }

    [Fact]
    public void ProcessCommandTrigger_LogsWhenACommandHasBeenExecutedUnsuccessfully()
    {
        // Arrange
        var commandTriggers = new List<CommandTrigger>
        {
            new()
            {
                ScreenCorner = ScreenCorner.TopLeft,
                Command = Testing.CommandThatRunsUnsuccessfully
            }
        };
        
        // Act
        this._commandTriggerService.ProcessCommandTrigger(commandTriggers, Testing.TopLeftCorner.X, Testing.TopLeftCorner.Y);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage("Could not run {Command}").Once()
            .WithLevel(LogEventLevel.Error)
            .WithProperty("Command").WithValue(Testing.CommandThatRunsUnsuccessfully);
    }

    [Fact]
    public void ProcessCommandTrigger_ExecutesCommandsWhenMouseCursorIsInCorrectCorner()
    {
        // Arrange
        var commandTriggers = new List<CommandTrigger>
        {
            new()
            {
                ScreenCorner = ScreenCorner.BottomRight,
                Command = Testing.CommandThatRunsSuccessfully
            },
            new()
            {
                ScreenCorner = ScreenCorner.TopLeft,
                Command = Testing.CommandThatRunsUnsuccessfully
            }
        };
        
        // Act
        this._commandTriggerService.ProcessCommandTrigger(commandTriggers, Testing.TopLeftCorner.X, Testing.TopLeftCorner.Y);

        // Assert
        this.VerifyCommandIsNeverExecuted(Testing.CommandThatRunsSuccessfully);
        this.VerifyCommandIsExecuted(Testing.CommandThatRunsUnsuccessfully);
    }

    private void VerifyCommandIsNeverExecuted(string command) =>
        this._processWrapperMock.Verify(pw =>
            pw.Start("cmd", $"/c {command}"), Times.Never);

    private void VerifyCommandIsExecuted(string command) =>
        this._processWrapperMock.Verify(pw =>
            pw.Start("cmd", $"/c {command}"), Times.Once);
}
