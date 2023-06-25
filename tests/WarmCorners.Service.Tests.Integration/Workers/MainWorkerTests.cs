using System.Reactive.Concurrency;
using Moq;
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
        var simpleReactiveGlobalHookWrapper = Testing.GetService<ISimpleReactiveGlobalHookWrapper>();
        var simpleReactiveGlobalHookWrapperMock = Mock.Get(simpleReactiveGlobalHookWrapper);

        // Act

        // Assert
        simpleReactiveGlobalHookWrapperMock.Verify(rgh => rgh.RunAsync(), Times.Once);
    }

    [Test]
    public void SimulatesCommandWhenUserMovesMouseOverCorner()
    {
        // Arrange
        var processWrapper = Testing.GetService<IProcessWrapper>();
        var processWrapperMock = Mock.Get(processWrapper);

        const string expectedCommand = "/c notepad";

        // Act
        Testing.TestScheduler.Schedule(() => Testing.TestMouseMovedSubject.OnNext(new MouseHookEventArgs(new UioHookEvent
        {
            Mouse = new MouseEventData
            {
                X = 1024,
                Y = 0
            }
        })));
        Testing.TestScheduler.Start();

        // Assert
        processWrapperMock.Verify(pw =>
                pw.Start("cmd", expectedCommand),
            Times.Once);
    }

    [Test]
    public void SimulatesKeyCombinationWhenUserMovesMouseOverCorner()
    {
        // Arrange
        var eventSimulatorWrapper = Testing.GetService<IEventSimulatorWrapper>();
        var eventSimulatorWrapperMock = Mock.Get(eventSimulatorWrapper);

        var expectedKeyCodes = new List<KeyCode>
        {
            KeyCode.VcLeftMeta,
            KeyCode.VcTab
        };

        // Act
        Testing.TestScheduler.Schedule(() => Testing.TestMouseMovedSubject.OnNext(new MouseHookEventArgs(new UioHookEvent
        {
            Mouse = new MouseEventData
            {
                X = 0,
                Y = 0
            }
        })));
        Testing.TestScheduler.Start();

        // Assert
        eventSimulatorWrapperMock.Verify(esw =>
                esw.SimulateKeyPress(It.Is<KeyCode>(kc => expectedKeyCodes.Contains(kc))),
            Times.Exactly(2));
        eventSimulatorWrapperMock.Verify(esw =>
                esw.SimulateKeyRelease(It.Is<KeyCode>(kc => expectedKeyCodes.Contains(kc))),
            Times.Exactly(2));
    }
}
