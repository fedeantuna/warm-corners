using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using Moq;
using SharpHook;
using SharpHook.Native;

namespace WarmCorners.Service.Tests.Integration;

public static class Testing
{
    private static readonly Subject<MouseHookEventArgs> TestMouseMovedSubject = new();
    private static readonly TestApplicationFactory TestApplicationFactory = new();

    public static (short Width, short Height) TestDisplaySize => (1024, 768);
    public static (short X, short Y) TopLeftCorner => (0, 0);
    public static (short X, short Y) TopRightCorner => (TestDisplaySize.Width, 0);
    public static IObservable<MouseHookEventArgs> TestMouseMoved { get; } = TestMouseMovedSubject.AsObservable();
    public static TestScheduler TestScheduler { get; } = new();

    public static Mock<T> GetRequiredServiceMock<T>()
        where T : class
    {
        var service = GetRequiredService<T>();
        var serviceMock = Mock.Get(service);

        return serviceMock;
    }

    public static void StartTestApplication() =>
        // Workaround to start the Test Application
        _ = TestApplicationFactory.Server;

    public static void TriggerMouseEvent(short x, short y)
    {
        TestScheduler.Schedule(() =>
            TestMouseMovedSubject.OnNext(new MouseHookEventArgs(new UioHookEvent
            {
                Mouse = new MouseEventData
                {
                    X = x,
                    Y = y
                }
            })));
        TestScheduler.Start();
    }

    private static T GetRequiredService<T>()
        where T : notnull =>
        TestApplicationFactory.Services.GetRequiredService<T>();
}
