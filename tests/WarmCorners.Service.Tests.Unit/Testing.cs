using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MediatR;
using Microsoft.Reactive.Testing;
using Moq;
using SharpHook;
using SharpHook.Native;
using WarmCorners.Service.Configurations;

namespace WarmCorners.Service.Tests.Unit;

public static class Testing
{
    private static readonly Subject<MouseHookEventArgs> TestMouseMovedSubject = new();

    public static IObservable<MouseHookEventArgs> TestMouseMoved { get; } = TestMouseMovedSubject.AsObservable();
    public static TestScheduler TestScheduler { get; } = new();

    public static TriggerConfiguration TriggerConfiguration => new()
    {
        KeyCombinationTriggers = new List<KeyCombinationTriggerConfiguration>
        {
            new()
            {
                ScreenCorner = "TopLeft",
                KeyCombination = "LeftMeta+Tab"
            }
        },
        ShellTriggers = new List<ShellTriggerConfiguration>
        {
            new()
            {
                ScreenCorner = "TopRight",
                ShellCommand = "notepad"
            }
        }
    };

    public static void TriggerMouseEvent()
    {
        TestScheduler.Schedule(() =>
            TestMouseMovedSubject.OnNext(new MouseHookEventArgs(new UioHookEvent
            {
                Mouse = new MouseEventData
                {
                    X = 0,
                    Y = 0
                }
            })));
        TestScheduler.Start();
    }

    public static void VerifyCommandIsCalledOnce<TCommand>(this Mock<ISender> senderMock)
        where TCommand : IBaseRequest =>
        senderMock.Verify(s =>
                s.Send(It.Is<TCommand>(br => br.GetType() == typeof(TCommand)),
                    It.IsAny<CancellationToken>()),
            Times.Once);

    public static void VerifyCommandIsNotCalled<TCommand>(this Mock<ISender> senderMock)
        where TCommand : IBaseRequest =>
        senderMock.Verify(s => s.Send(It.IsAny<TCommand>(), It.IsAny<CancellationToken>()), Times.Never);
}
