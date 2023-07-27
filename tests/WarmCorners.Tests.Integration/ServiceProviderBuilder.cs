using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Sinks.InMemory;
using SharpHook;
using WarmCorners.Application;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Infrastructure;
using WarmCorners.Infrastructure.Wrappers;

namespace WarmCorners.Tests.Integration;

public class ServiceProviderBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public ServiceProviderBuilder()
    {
        this._services.AddApplicationServices()
            .AddInfrastructureServices();

        this.ReplaceWrappersWithMocks();

        this.SetupInMemoryLogger();
    }

    public IServiceProvider Build() => this._services.BuildServiceProvider();

    private void ReplaceWrappersWithMocks()
    {
        this._services.ReplaceServiceWithMock<IDateTimeOffsetWrapper>(ServiceLifetime.Transient);
        this._services.ReplaceServiceWithMock<IProcessWrapper>(ServiceLifetime.Transient);
        this._services.ReplaceServiceWithMock<IEventSimulator>(ServiceLifetime.Singleton);

        var schedulerWrapperMock = this._services.ReplaceServiceWithMock<ISchedulerWrapper>(ServiceLifetime.Transient);
        schedulerWrapperMock.SetupGet(sw => sw.Default).Returns(Testing.TestScheduler);

        var user32WrapperMock = this._services.ReplaceServiceWithMock<IUser32Wrapper>(ServiceLifetime.Transient);
        user32WrapperMock.Setup(u32W =>
            u32W.GetScreenResolution()).Returns((Testing.TestDisplaySize.Width, Testing.TestDisplaySize.Height));
    }

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
    public static Mock<TService> ReplaceServiceWithMock<TService>(this IServiceCollection services, ServiceLifetime serviceLifetime)
        where TService : class
    {
        var service = services.Single(sd => sd.ServiceType == typeof(TService));
        services.Remove(service);
        var replace = new Mock<TService>();
        var serviceDescriptor = new ServiceDescriptor(typeof(TService), _ => replace.Object, serviceLifetime);
        services.Add(serviceDescriptor);

        return replace;
    }
}
