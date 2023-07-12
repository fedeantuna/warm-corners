using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using SharpHook;
using SharpHook.Native;
using WarmCorners.Application.KeyCombinationTriggers.Commands;
using WarmCorners.Domain.Enums;

namespace WarmCorners.Application.Tests.Unit.KeyCombinationTriggers.Commands;

public class ProcessKeyCombinationTriggerCommandTests
{
    private readonly Mock<IEventSimulator> _eventSimulatorMock;

    private readonly ISender _sender;

    public ProcessKeyCombinationTriggerCommandTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        this._sender = provider.GetRequiredService<ISender>();

        var eventSimulator = provider.GetRequiredService<IEventSimulator>();
        this._eventSimulatorMock = Mock.Get(eventSimulator);
    }

    [Fact]
    public async Task ProcessKeyCombinationTriggerCommandHandler_LogsWhenAKeyCombinationHasBeenExecutedCorrectly()
    {
        // Arrange
        var keyCodes = new List<KeyCode>
        {
            KeyCode.VcLeftMeta,
            KeyCode.VcTab
        };
        var processKeyCombinationTriggerCommand = new ProcessKeyCombinationTriggerCommand
        {
            ScreenCorner = ScreenCorner.TopLeft,
            KeyCombination = keyCodes,
            Position = Testing.TopLeftCorner
        };

        // Act
        await this._sender.Send(processKeyCombinationTriggerCommand);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage(ProcessKeyCombinationTriggerCommandHandler.ExecutedKeyCombinationLogMessageTemplate).Once()
            .WithLevel(LogEventLevel.Information)
            .WithProperty("KeyCombination").WithValue(string.Join('+', keyCodes.Select(k => k.ToString())));
    }

    [Fact]
    public void ProcessKeyCombinationCommandTriggerHandler_DoesNothingWhenTheKeyCombinationIsEmpty()
    {
        // Arrange
        var processKeyCombinationTriggerCommand = new ProcessKeyCombinationTriggerCommand
        {
            ScreenCorner = ScreenCorner.TopLeft,
            KeyCombination = Enumerable.Empty<KeyCode>().ToList(),
            Position = Testing.TopLeftCorner
        };

        // Act
        this._sender.Send(processKeyCombinationTriggerCommand);

        // Assert
        this.VerifyNoKeyCombinationIsExecuted();
    }

    [Fact]
    public async Task ProcessKeyCombinationTriggerCommandHandler_ExecutesKeyCombinationWhenMouseCursorIsInCorrectCorner()
    {
        // Arrange
        var keyCodes = new List<KeyCode>
        {
            KeyCode.VcLeftMeta,
            KeyCode.VcTab
        };
        var processKeyCombinationTriggerCommand = new ProcessKeyCombinationTriggerCommand
        {
            ScreenCorner = ScreenCorner.TopLeft,
            KeyCombination = keyCodes,
            Position = Testing.TopLeftCorner
        };

        // Act
        await this._sender.Send(processKeyCombinationTriggerCommand);

        // Assert
        this.VerifyKeyCombinationIsExecuted(keyCodes);
    }

    [Fact]
    public async Task ProcessKeyCombinationTriggerCommandHandler_DoesNotExecuteKeyCombinationWhenMouseCursorIsNotInCorrectCorner()
    {
        // Arrange
        var keyCodes = new List<KeyCode>
        {
            KeyCode.VcLeftMeta,
            KeyCode.VcTab
        };
        var processKeyCombinationTriggerCommand = new ProcessKeyCombinationTriggerCommand
        {
            ScreenCorner = ScreenCorner.BottomRight,
            KeyCombination = keyCodes,
            Position = Testing.TopLeftCorner
        };

        // Act
        await this._sender.Send(processKeyCombinationTriggerCommand);

        // Assert
        this.VerifyKeyCombinationIsNeverExecuted(keyCodes);
    }

    private void VerifyNoKeyCombinationIsExecuted()
    {
        this._eventSimulatorMock.Verify(esw =>
            esw.SimulateKeyPress(It.IsAny<KeyCode>()), Times.Never);
        this._eventSimulatorMock.Verify(esw =>
            esw.SimulateKeyRelease(It.IsAny<KeyCode>()), Times.Never);
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

    private void VerifyKeyCombinationIsNeverExecuted(IEnumerable<KeyCode> keyCodes)
    {
        this._eventSimulatorMock.Verify(esw =>
            esw.SimulateKeyPress(It.Is<KeyCode>(kc =>
                keyCodes.Contains(kc))), Times.Never);
        this._eventSimulatorMock.Verify(esw =>
            esw.SimulateKeyRelease(It.Is<KeyCode>(kc =>
                keyCodes.Contains(kc))), Times.Never);
    }
}
