using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using SharpHook.Native;
using WarmCorners.Core.Common;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Core.Wrappers;

namespace WarmCorners.Core.Tests.Unit.Services;

public class KeyCombinationTriggerServiceTests
{
    private readonly IKeyCombinationTriggerService _keyCombinationTriggerService;
    private readonly Mock<IEventSimulatorWrapper> _eventSimulatorWrapperMock;
    private readonly Mock<IScreenService> _screenServiceMock;

    public KeyCombinationTriggerServiceTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        var eventSimulatorWrapper = provider.GetRequiredService<IEventSimulatorWrapper>();
        this._eventSimulatorWrapperMock = Mock.Get(eventSimulatorWrapper);
        var screenService = provider.GetRequiredService<IScreenService>();
        this._screenServiceMock = Mock.Get(screenService);

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
        const int topLeftCornerX = 0;
        const int topLeftCornerY = 0;
        var keyCombinationTriggers = new List<KeyCombinationTrigger>
        {
            new()
            {
                ScreenCorner = ScreenCorner.TopLeft,
                KeyCodes = keyCodes
            }
        };

        this._screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(ScreenCorner.TopLeft, topLeftCornerX, topLeftCornerY))
            .Returns(true);

        // Act
        this._keyCombinationTriggerService.ProcessKeyCombinationTrigger(keyCombinationTriggers, topLeftCornerX, topLeftCornerY);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage("Executed {KeyCombination}").Once()
            .WithLevel(LogEventLevel.Information)
            .WithProperty("KeyCombination").WithValue(string.Join('+', keyCodes.Select(k => k.ToString())));
    }

    [Fact]
    public void ProcessCommandTrigger_ExecutesCommandsWhenMouseCursorIsInCorrectCorner()
    {
        // Arrange
        var topRightCornerKeys = new List<KeyCode>
        {
            KeyCode.VcRightAlt,
            KeyCode.VcRightControl,
            KeyCode.VcDelete
        };
        var bottomRightKeys = new List<KeyCode>
        {
            KeyCode.VcLeftAlt,
            KeyCode.VcTab
        };
        var bottomLeftKeys = new List<KeyCode>
        {
            KeyCode.VcLeftControl,
            KeyCode.VcLeftShift,
            KeyCode.VcBackquote
        };
        
        const int bottomRightCornerX = 768;
        const int bottomRightCornerY = 1024;
        var keyCombinationTriggers = new List<KeyCombinationTrigger>
        {
            new()
            {
                ScreenCorner = ScreenCorner.TopRight,
                KeyCodes = topRightCornerKeys
            },
            new()
            {
                ScreenCorner = ScreenCorner.BottomRight,
                KeyCodes = bottomRightKeys
            },
            new()
            {
                ScreenCorner = ScreenCorner.BottomLeft,
                KeyCodes = bottomLeftKeys
            }
        };
        
        this._screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(ScreenCorner.TopRight, bottomRightCornerX, bottomRightCornerY))
            .Returns(false);
        this._screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(ScreenCorner.BottomRight, bottomRightCornerX, bottomRightCornerY))
            .Returns(true);
        this._screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(ScreenCorner.BottomLeft, bottomRightCornerX, bottomRightCornerY))
            .Returns(false);
        
        // Act
        this._keyCombinationTriggerService.ProcessKeyCombinationTrigger(keyCombinationTriggers, bottomRightCornerX, bottomRightCornerY);
        
        // Assert
        this._eventSimulatorWrapperMock.Verify(esw =>
                esw.SimulateKeyPress(It.Is<KeyCode>(kc =>
                    topRightCornerKeys.Contains(kc))),
            Times.Never);
        this._eventSimulatorWrapperMock.Verify(esw =>
                esw.SimulateKeyRelease(It.Is<KeyCode>(kc =>
                    topRightCornerKeys.Contains(kc))),
            Times.Never);
        this._eventSimulatorWrapperMock.Verify(esw =>
                esw.SimulateKeyPress(It.Is<KeyCode>(kc =>
                    bottomRightKeys.Contains(kc))),
            Times.Exactly(bottomRightKeys.Count));
        this._eventSimulatorWrapperMock.Verify(esw =>
                esw.SimulateKeyRelease(It.Is<KeyCode>(kc =>
                    bottomRightKeys.Contains(kc))),
            Times.Exactly(bottomRightKeys.Count));
        this._eventSimulatorWrapperMock.Verify(esw =>
                esw.SimulateKeyPress(It.Is<KeyCode>(kc =>
                    bottomLeftKeys.Contains(kc))),
            Times.Never);
        this._eventSimulatorWrapperMock.Verify(esw =>
                esw.SimulateKeyRelease(It.Is<KeyCode>(kc =>
                    bottomLeftKeys.Contains(kc))),
            Times.Never);
    }
}
