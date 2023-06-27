using System.Diagnostics;
using Moq;
using SharpHook;
using SharpHook.Native;
using SharpHook.Reactive;
using WarmCorners.Core.Wrappers;

namespace WarmCorners.Service.Tests.Integration.Workers;

public class MainWorkerTests
{
    [Test]
    public void StartsListeningHooks()
    {
        // Arrange
        var reactiveGlobalHookMock = Testing.GetRequiredServiceMock<IReactiveGlobalHook>();

        // Act

        // Assert
        reactiveGlobalHookMock.Verify(rgh =>
            rgh.RunAsync(), Times.Once);
    }

    [Test]
    public void ExecutesCommandWhenUserMovesMouseOverCorner()
    {
        // Arrange
        const string expectedCommand = "notepad";

        // Act
        Testing.TriggerMouseEvent(Testing.TopRightCorner.X, Testing.TopRightCorner.Y);

        // Assert
        VerifyCommandIsExecutedOnce(expectedCommand);
    }

    [Test]
    public void ExecutesKeyCombinationWhenUserMovesMouseOverCorner()
    {
        // Arrange
        var expectedKeyCodes = new List<KeyCode>
        {
            KeyCode.VcLeftMeta,
            KeyCode.VcTab
        };

        // Act
        Testing.TriggerMouseEvent(Testing.TopLeftCorner.X, Testing.TopLeftCorner.Y);

        // Assert
        VerifyKeyCombinationIsExecutedOnce(expectedKeyCodes);
    }

    private static void VerifyCommandIsExecutedOnce(string command)
    {
        var processWrapperMock = Testing.GetRequiredServiceMock<IProcessWrapper>();
        processWrapperMock.Verify(pw =>
            pw.SetStartInfo(It.Is<ProcessStartInfo>(psi =>
                psi.Arguments == $"/c {command}")), Times.Once);
        processWrapperMock.Verify(pw =>
            pw.Start(), Times.Once);
    }

    private static void VerifyKeyCombinationIsExecutedOnce(IEnumerable<KeyCode> expectedKeyCodes)
    {
        var eventSimulatorMock = Testing.GetRequiredServiceMock<IEventSimulator>();
        eventSimulatorMock.Verify(esw =>
            esw.SimulateKeyPress(It.Is<KeyCode>(kc =>
                expectedKeyCodes.Contains(kc))), Times.Exactly(2));
        eventSimulatorMock.Verify(esw =>
            esw.SimulateKeyRelease(It.Is<KeyCode>(kc =>
                expectedKeyCodes.Contains(kc))), Times.Exactly(2));
    }
}
