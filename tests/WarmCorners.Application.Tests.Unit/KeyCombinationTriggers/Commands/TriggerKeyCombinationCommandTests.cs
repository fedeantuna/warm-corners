using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using SharpHook;
using SharpHook.Native;
using WarmCorners.Application.KeyCombinationTriggers.Commands.TriggerKeyCombination;

namespace WarmCorners.Application.Tests.Unit.KeyCombinationTriggers.Commands;

public class TriggerKeyCombinationCommandTests
{
    private readonly Mock<IEventSimulator> _eventSimulatorMock;

    private readonly ISender _sender;

    public TriggerKeyCombinationCommandTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        this._sender = provider.GetRequiredService<ISender>();

        var eventSimulator = provider.GetRequiredService<IEventSimulator>();
        this._eventSimulatorMock = Mock.Get(eventSimulator);
    }

    [Fact]
    public async Task TriggerKeyCombinationCommandHandler_LogsWhenAKeyCombinationHasBeenExecutedCorrectly()
    {
        // Arrange
        var keyCodes = new List<KeyCode>
        {
            KeyCode.VcLeftMeta,
            KeyCode.VcTab
        };
        var triggerKeyCombinationCommand = new TriggerKeyCombinationCommand
        {
            KeyCombination = keyCodes
        };

        // Act
        await this._sender.Send(triggerKeyCombinationCommand);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage(TriggerKeyCombinationCommandHandler.ExecutedKeyCombinationLogMessageTemplate).Once()
            .WithLevel(LogEventLevel.Information)
            .WithProperty("KeyCombination").WithValue(string.Join('+', keyCodes.Select(k => k.ToString())));
    }

    [Fact]
    public async Task TriggerKeyCombinationCommandHandler_ExecutesKeyCombinationWhenMouseCursorIsInCorrectCorner()
    {
        // Arrange
        var keyCodes = new List<KeyCode>
        {
            KeyCode.VcLeftMeta,
            KeyCode.VcTab
        };
        var triggerKeyCombinationCommand = new TriggerKeyCombinationCommand
        {
            KeyCombination = keyCodes
        };

        // Act
        await this._sender.Send(triggerKeyCombinationCommand);

        // Assert
        this.VerifyKeyCombinationIsExecuted(keyCodes);
    }

    [Fact]
    public async Task TriggerKeyCombinationCommandHandler_LogsErrorWhenKeyCombinationListIsEmpty()
    {
        // Arrange
        var triggerKeyCombinationCommand = new TriggerKeyCombinationCommand
        {
            KeyCombination = new List<KeyCode>()
        };

        // Act
        await this._sender.Send(triggerKeyCombinationCommand);

        // Assert
        VerifyTriggerKeyCombinationCommandValidationExceptionHandlerLog("[]");
    }

    [Fact]
    public async Task TriggerKeyCombinationCommandHandler_LogsErrorWhenKeyCombinationListIsNull()
    {
        // Arrange
        var triggerKeyCombinationCommand = new TriggerKeyCombinationCommand
        {
            KeyCombination = null!
        };

        // Act
        await this._sender.Send(triggerKeyCombinationCommand);

        // Assert
        VerifyTriggerKeyCombinationCommandValidationExceptionHandlerLog(null!);
    }

    private void VerifyKeyCombinationIsExecuted(IReadOnlyCollection<KeyCode> keyCodes)
    {
        this._eventSimulatorMock.Verify(esw =>
            esw.SimulateKeyPress(It.Is<KeyCode>(kc =>
                keyCodes.Contains(kc))), Times.Exactly(keyCodes.Count));
        this._eventSimulatorMock.Verify(esw =>
            esw.SimulateKeyRelease(It.Is<KeyCode>(kc =>
                keyCodes.Contains(kc))), Times.Exactly(keyCodes.Count));
    }

    private static void VerifyTriggerKeyCombinationCommandValidationExceptionHandlerLog(object expectedKeyCombination) =>
        InMemorySink.Instance
            .Should()
            .HaveMessage(TriggerKeyCombinationCommandValidationExceptionHandler.RequestValidationExceptionLogMessageTemplate).Once()
            .WithLevel(LogEventLevel.Error)
            .WithProperty("RequestName").WithValue(nameof(TriggerKeyCombinationCommand))
            .And.WithProperty("Request").HavingADestructuredObject().WithProperty("KeyCombination").WithValue(expectedKeyCombination)
            .And.WithProperty("Errors");
}
