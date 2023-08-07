using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
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
}
