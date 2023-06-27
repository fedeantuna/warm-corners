using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Sinks.InMemory;
using SharpHook;
using WarmCorners.Core.Common;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Core.Wrappers;

namespace WarmCorners.Core.Tests.Unit;

public class ServiceProviderBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public ServiceProviderBuilder()
    {
        this._services.AddCoreServices();

        this.ReplaceWrappersWithMocks();

        this.AddInfrastructureServiceMocks();

        this.SetupEventSimulatorMock();
        this.SetupInMemoryLogger();
    }

    public IServiceProvider Build() =>
        this._services.BuildServiceProvider();

    private void ReplaceWrappersWithMocks() =>
        this._services.ReplaceServiceWithMock<IProcessWrapper>();

    private void AddInfrastructureServiceMocks()
    {
        var screenServiceMock = new Mock<IScreenService>();
        screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(ScreenCorner.TopLeft, 0, 0))
            .Returns(true);
        screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(ScreenCorner.TopRight, Testing.TestDisplaySize.Width, 0))
            .Returns(false);
        screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(ScreenCorner.BottomRight, Testing.TestDisplaySize.Width, Testing.TestDisplaySize.Height))
            .Returns(false);
        screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(ScreenCorner.BottomLeft, 0, Testing.TestDisplaySize.Height))
            .Returns(false);
        this._services.AddSingleton(_ => screenServiceMock.Object);
    }

    private void SetupEventSimulatorMock() =>
        this._services.ReplaceServiceWithMock<IEventSimulator>();

    private void SetupInMemoryLogger() =>
        this._services.AddLogging(builder =>
        {
            builder.ClearProviders();

            var logger = new LoggerConfiguration()
                .WriteTo.InMemory()
                .MinimumLevel.Verbose()
                .CreateLogger();
            builder.AddSerilog(logger);
        });
}

public static class ServiceCollectionExtensions
{
    public static void ReplaceServiceWithMock<TIService>(this IServiceCollection services)
        where TIService : class
    {
        var service = services.Single(sd => sd.ServiceType == typeof(TIService));
        services.Remove(service);
        var replace = new Mock<TIService>();
        services.AddSingleton(_ => replace.Object);
    }
}
