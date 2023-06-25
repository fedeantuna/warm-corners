using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using Moq;
using SharpHook;

namespace WarmCorners.Service.Tests.Integration;

public static class Testing
{
    private static readonly TestApplicationFactory TestApplicationFactory = new();

    public static (int Width, int Height) TestDisplaySize => (1024, 768);
    public static IObservable<MouseHookEventArgs> TestMouseMoved => TestMouseMovedSubject.AsObservable();
    public static Subject<MouseHookEventArgs> TestMouseMovedSubject { get; } = new();
    public static TestScheduler TestScheduler { get; } = new();

    public static T GetService<T>()
        where T : notnull =>
        TestApplicationFactory.Services.GetRequiredService<T>();

    public static Mock<TIService> ReplaceServiceWithMock<TIService>(this IServiceCollection services)
        where TIService : class
    {
        var service = services.Single(sd => sd.ServiceType == typeof(TIService));
        services.Remove(service);
        var replace = new Mock<TIService>();
        services.AddSingleton(_ => replace.Object);

        return replace;
    }
}
