using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using SharpHook;
using SharpHook.Native;
using WarmCorners.Core.Common;
using WarmCorners.Core.Services.Abstractions;

namespace WarmCorners.Core.Tests.Unit.Services;

public class KeyCombinationTriggerServiceTests
{
    private readonly Mock<IEventSimulator> _eventSimulatorMock;
    private readonly IKeyCombinationTriggerService _keyCombinationTriggerService;

    public KeyCombinationTriggerServiceTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        var eventSimulator = provider.GetRequiredService<IEventSimulator>();
        this._eventSimulatorMock = Mock.Get(eventSimulator);

        this._keyCombinationTriggerService = provider.GetRequiredService<IKeyCombinationTriggerService>();
    }

    [Fact]
    public void ProcessKeyCombinationTrigger_LogsWhenAKeyCombinationHasBeenExecutedCorrectly()
    {
        // Arrange
        var keyCodes = new List<KeyCode>
        {
            KeyCode.VcLeftMeta,
            KeyCode.VcTab
        };
        var keyCombinationTriggers = new List<KeyCombinationTrigger>
        {
            new()
            {
                ScreenCorner = ScreenCorner.TopLeft,
                KeyCodes = keyCodes
            }
        };

        // Act
        this._keyCombinationTriggerService.ProcessKeyCombinationTrigger(keyCombinationTriggers,
            Testing.TopLeftCorner.X,
            Testing.TopLeftCorner.Y);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage("Executed {KeyCombination}").Once()
            .WithLevel(LogEventLevel.Information)
            .WithProperty("KeyCombination").WithValue(string.Join('+', keyCodes.Select(k => k.ToString())));
    }

    [Fact]
    public void ProcessKeyCombinationTrigger_DoesNothingWhenTheKeyCodeListIsEmpty()
    {
        // Arrange
        var keyCombinationTriggers = new List<KeyCombinationTrigger>
        {
            new()
            {
                ScreenCorner = ScreenCorner.TopLeft,
                KeyCodes = new List<KeyCode>()
            }
        };

        // Act
        this._keyCombinationTriggerService.ProcessKeyCombinationTrigger(keyCombinationTriggers,
            Testing.TopLeftCorner.X,
            Testing.TopLeftCorner.Y);

        // Assert
        this.VerifyNoKeyCombinationIsExecuted();
    }

    [Fact]
    public void ProcessKeyCombinationTrigger_ExecutesKeyCombinationWhenMouseCursorIsInCorrectCorner()
    {
        // Arrange
        var bottomLeftKeys = new List<KeyCode>
        {
            KeyCode.VcLeftControl,
            KeyCode.VcLeftShift,
            KeyCode.VcBackquote
        };
        var topLeftKeys = new List<KeyCode>
        {
            KeyCode.VcLeftAlt,
            KeyCode.VcTab
        };
        var keyCombinationTriggers = new List<KeyCombinationTrigger>
        {
            new()
            {
                ScreenCorner = ScreenCorner.BottomLeft,
                KeyCodes = bottomLeftKeys
            },
            new()
            {
                ScreenCorner = ScreenCorner.TopLeft,
                KeyCodes = topLeftKeys
            }
        };

        // Act
        this._keyCombinationTriggerService.ProcessKeyCombinationTrigger(keyCombinationTriggers,
            Testing.TopLeftCorner.X,
            Testing.TopLeftCorner.Y);

        // Assert
        this.VerifyKeyCombinationIsNeverExecuted(bottomLeftKeys);
        this.VerifyKeyCombinationIsExecuted(topLeftKeys);
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

    private void VerifyNoKeyCombinationIsExecuted()
    {
        this._eventSimulatorMock.Verify(esw =>
            esw.SimulateKeyPress(It.IsAny<KeyCode>()), Times.Never);
        this._eventSimulatorMock.Verify(esw =>
            esw.SimulateKeyRelease(It.IsAny<KeyCode>()), Times.Never);
    }
}
