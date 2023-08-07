using System.Reactive.Linq;
using System.Reactive.Subjects;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using Serilog.Sinks.InMemory;
using SharpHook.Reactive;
using WarmCorners.Application.Common.Services;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Service.Configurations;

namespace WarmCorners.Service.Tests.Unit;

public class ServiceProviderBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public ServiceProviderBuilder()
    {
        this._services.AddPresentationServices(new ConfigurationBuilder().Build());

        this.AddApplicationMocks();
        this.AddInfrastructureMocks();

        this.AddSettings();

        this.SetupReactiveGlobalHookMock();
        this.SetupInMemoryLogger();
    }

    public IServiceProvider Build() => this._services.BuildServiceProvider();

    private void AddApplicationMocks()
    {
        var senderMock = new Mock<ISender>();
        this._services.AddTransient(_ => senderMock.Object);
    }

    private void AddInfrastructureMocks()
    {
        var screenServiceMock = new Mock<IScreenService>();
        this._services.AddSingleton(_ => screenServiceMock.Object);

        var schedulerWrapperMock = new Mock<ISchedulerWrapper>();
        schedulerWrapperMock.Setup(sw => sw.Default).Returns(Testing.TestScheduler);
        this._services.AddTransient(_ => schedulerWrapperMock.Object);
    }

    private void AddSettings()
    {
        var triggerConfigurationOptionsMonitor = Mock.Of<IOptionsMonitor<TriggerConfiguration>>(om =>
            om.CurrentValue == Testing.TriggerConfiguration);
        this._services.AddSingleton(_ => triggerConfigurationOptionsMonitor);
    }

    private void SetupReactiveGlobalHookMock()
    {
        var reactiveGlobalHookMock = this._services.ReplaceServiceWithMock<IReactiveGlobalHook>(ServiceLifetime.Singleton);
        reactiveGlobalHookMock
            .Setup(rgh => rgh.MouseMoved)
            .Returns(Testing.TestMouseMoved);
        reactiveGlobalHookMock
            .Setup(rgh => rgh.RunAsync())
            .Returns(new Subject<System.Reactive.Unit>().AsObservable());
        reactiveGlobalHookMock
            .Setup(rgh => rgh.Dispose())
            .Callback(() => { });
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
