using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using WarmCorners.Application.Common.Services;
using WarmCorners.Application.ShellTriggers.Commands;
using WarmCorners.Domain.Enums;

namespace WarmCorners.Application.Tests.Unit.ShellTriggers.Commands;

public class ProcessShellTriggerCommandTests
{
    private readonly ISender _sender;
    private readonly Mock<IShellService> _shellServiceMock;

    public ProcessShellTriggerCommandTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        this._sender = provider.GetRequiredService<ISender>();

        var shellService = provider.GetRequiredService<IShellService>();
        this._shellServiceMock = Mock.Get(shellService);
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
    public async Task ProcessShellTriggerCommandHandler_ExecutesShellCommandWhenMouseCursorIsInCorrectCorner()
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
        this._shellServiceMock.Verify(ss => ss.Run(Testing.SomeShellCommand), Times.Once);
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
        this._shellServiceMock.Verify(ss => ss.Run(Testing.SomeShellCommand), Times.Never);
    }
}
