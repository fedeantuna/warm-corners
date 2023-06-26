using System.Reactive.Concurrency;
using Moq;
using NUnit.Framework.Internal;
using SharpHook;
using SharpHook.Native;
using WarmCorners.Core.Wrappers;
using WarmCorners.Service.Wrappers;

namespace WarmCorners.Service.Tests.Integration.Workers;

public class MainWorkerTests
{
    [Test]
    public void StartsListeningHooks()
    {
        // Arrange
        var simpleReactiveGlobalHookWrapperMock = Testing.GetRequiredServiceMock<ISimpleReactiveGlobalHookWrapper>();

        // Act

        // Assert
        simpleReactiveGlobalHookWrapperMock.Verify(rgh =>
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
            pw.Start("cmd", $"/c {command}"), Times.Once);
    }

    private static void VerifyKeyCombinationIsExecutedOnce(IEnumerable<KeyCode> expectedKeyCodes)
    {
        var eventSimulatorWrapperMock = Testing.GetRequiredServiceMock<IEventSimulatorWrapper>();
        eventSimulatorWrapperMock.Verify(esw =>
                esw.SimulateKeyPress(It.Is<KeyCode>(kc =>
                    expectedKeyCodes.Contains(kc))), Times.Exactly(2));
        eventSimulatorWrapperMock.Verify(esw =>
                esw.SimulateKeyRelease(It.Is<KeyCode>(kc =>
                    expectedKeyCodes.Contains(kc))), Times.Exactly(2));
    }
}
