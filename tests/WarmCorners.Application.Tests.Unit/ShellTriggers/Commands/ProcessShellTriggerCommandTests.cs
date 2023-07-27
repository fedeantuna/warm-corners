using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Application.ShellTriggers.Commands;
using WarmCorners.Domain.Enums;

namespace WarmCorners.Application.Tests.Unit.ShellTriggers.Commands;

public class ProcessShellTriggerCommandTests
{
    private readonly Mock<IProcessWrapper> _processWrapperMock;
    private readonly ISender _sender;

    public ProcessShellTriggerCommandTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        this._sender = provider.GetRequiredService<ISender>();

        var processWrapper = provider.GetRequiredService<IProcessWrapper>();
        this._processWrapperMock = Mock.Get(processWrapper);
    }

    [Fact]
    public async Task ProcessShellTriggerCommandHandler_LogsWhenACommandHasBeenExecutedCorrectly()
    {
        // Arrange
        var processShellTriggerCommand = new ProcessShellTriggerCommand
        {
            ScreenCorner = ScreenCorner.TopLeft,
            ShellCommand = Testing.SomeShellCommand,
            Position = Testing.TopLeftCorner
        };

        this._processWrapperMock.Setup(pw => pw.Start()).Returns(true);

        // Act
        await this._sender.Send(processShellTriggerCommand);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage(ProcessShellTriggerCommandHandler.ExecutedShellCommandLogMessageTemplate).Once()
            .WithLevel(LogEventLevel.Information)
            .WithProperty("ShellCommand").WithValue(Testing.SomeShellCommand);
    }

    [Fact]
    public async Task ProcessShellTriggerCommandHandler_LogsWhenACommandHasNotBeenExecutedCorrectly()
    {
        // Arrange
        var processShellTriggerCommand = new ProcessShellTriggerCommand
        {
            ScreenCorner = ScreenCorner.TopLeft,
            ShellCommand = Testing.SomeShellCommand,
            Position = Testing.TopLeftCorner
        };

        // Act
        await this._sender.Send(processShellTriggerCommand);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage(ProcessShellTriggerCommandHandler.ErrorExecutingShellCommandLogMessageTemplate).Once()
            .WithLevel(LogEventLevel.Error)
            .WithProperty("ShellCommand").WithValue(Testing.SomeShellCommand);
    }

    [Fact]
    public async Task ProcessShellTriggerCommandHandler_ExecutesShellCommandWhenMouseCursorIsInCorrectCorner()
    {
        // Arrange
        var processShellTriggerCommand = new ProcessShellTriggerCommand
        {
            ScreenCorner = ScreenCorner.TopLeft,
            ShellCommand = Testing.SomeShellCommand,
            Position = Testing.TopLeftCorner
        };

        this._processWrapperMock.Setup(pw => pw.Start()).Returns(true);

        // Act
        await this._sender.Send(processShellTriggerCommand);

        // Assert
        this._processWrapperMock.Verify(ss => ss.Start(), Times.Once);
    }

    [Fact]
    public async Task ProcessShellTriggerCommandHandler_DoesNotExecuteShellCommandWhenMouseCursorIsNotInCorrectCorner()
    {
        // Arrange
        var processShellTriggerCommand = new ProcessShellTriggerCommand
        {
            ScreenCorner = ScreenCorner.TopRight,
            ShellCommand = Testing.SomeShellCommand,
            Position = Testing.TopLeftCorner
        };

        // Act
        await this._sender.Send(processShellTriggerCommand);

        // Assert
        this._processWrapperMock.Verify(ss => ss.Start(), Times.Never);
    }
}
