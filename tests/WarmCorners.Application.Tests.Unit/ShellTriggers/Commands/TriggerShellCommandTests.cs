using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Application.ShellTriggers.Commands.TriggerShell;

namespace WarmCorners.Application.Tests.Unit.ShellTriggers.Commands;

public class TriggerShellCommandTests
{
    private readonly Mock<IProcessWrapper> _processWrapperMock;
    private readonly ISender _sender;

    public TriggerShellCommandTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        this._sender = provider.GetRequiredService<ISender>();

        var processWrapper = provider.GetRequiredService<IProcessWrapper>();
        this._processWrapperMock = Mock.Get(processWrapper);
    }

    [Fact]
    public async Task TriggerShellCommandHandler_LogsWhenACommandHasBeenExecutedCorrectly()
    {
        // Arrange
        var triggerShellCommand = new TriggerShellCommand
        {
            ShellCommand = Testing.SomeShellCommand
        };

        this._processWrapperMock.Setup(pw => pw.Start()).Returns(true);

        // Act
        await this._sender.Send(triggerShellCommand);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage(TriggerShellCommandHandler.ExecutedShellCommandLogMessageTemplate).Once()
            .WithLevel(LogEventLevel.Information)
            .WithProperty("ShellCommand").WithValue(Testing.SomeShellCommand);
    }

    [Fact]
    public async Task TriggerShellCommandHandler_LogsWhenACommandHasNotBeenExecutedCorrectly()
    {
        // Arrange
        var triggerShellCommand = new TriggerShellCommand
        {
            ShellCommand = Testing.SomeShellCommand
        };

        // Act
        await this._sender.Send(triggerShellCommand);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage(TriggerShellCommandHandler.ErrorExecutingShellCommandLogMessageTemplate).Once()
            .WithLevel(LogEventLevel.Error)
            .WithProperty("ShellCommand").WithValue(Testing.SomeShellCommand);
    }

    [Fact]
    public async Task TriggerShellCommandHandler_ExecutesShellCommandWhenMouseCursorIsInCorrectCorner()
    {
        // Arrange
        var triggerShellCommand = new TriggerShellCommand
        {
            ShellCommand = Testing.SomeShellCommand
        };

        this._processWrapperMock.Setup(pw => pw.Start()).Returns(true);

        // Act
        await this._sender.Send(triggerShellCommand);

        // Assert
        this._processWrapperMock.Verify(ss => ss.Start(), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("       ")]
    [InlineData(null)]
    public async Task TriggerShellCommandHandler_LogsErrorWhenShellCommandIsEmptyOrWhiteSpaceOrNull(string shellCommand)
    {
        // Arrange
        var triggerShellCommand = new TriggerShellCommand
        {
            ShellCommand = shellCommand
        };

        // Act
        await this._sender.Send(triggerShellCommand);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage(TriggerShellCommandValidationExceptionHandler.RequestValidationExceptionLogMessageTemplate).Once()
            .WithLevel(LogEventLevel.Error)
            .WithProperty("RequestName").WithValue(nameof(TriggerShellCommand))
            .And.WithProperty("Request").HavingADestructuredObject().WithProperty("ShellCommand").WithValue(shellCommand)
            .And.WithProperty("Errors");
    }
}
