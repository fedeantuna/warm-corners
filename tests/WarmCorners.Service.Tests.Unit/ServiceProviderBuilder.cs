using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Sinks.InMemory;
using SharpHook.Reactive;
using WarmCorners.Application.Common.Wrappers;

namespace WarmCorners.Service.Tests.Unit;

public class ServiceProviderBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public ServiceProviderBuilder()
    {
        this._services.AddPresentationServices(new ConfigurationBuilder().Build());

        this.AddApplicationMocks();

        this.SetupReactiveGlobalHookMock();
        this.SetupInMemoryLogger();
    }

    public IServiceProvider Build() => this._services.BuildServiceProvider();

    private void AddApplicationMocks()
    {
        var senderMock = new Mock<ISender>();
        this._services.AddTransient(_ => senderMock.Object);

        var schedulerWrapperMock = new Mock<ISchedulerWrapper>();
        this._services.AddSingleton(_ => schedulerWrapperMock.Object);
    }

    private void SetupReactiveGlobalHookMock() =>
        this._services.ReplaceServiceWithMock<IReactiveGlobalHook>();

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
