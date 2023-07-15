using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Sinks.InMemory;
using SharpHook;
using WarmCorners.Application.Common.Services;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Domain.Enums;

namespace WarmCorners.Application.Tests.Unit;

public class ServiceProviderBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public ServiceProviderBuilder()
    {
        this._services.AddApplicationServices();
        this.AddFakeValidators();
        this.AddFakeMediatorRequests();

        this.ReplaceServicesWithMocks();

        this.SetupInMemoryLogger();
        this.AddInfrastructureWrapperMocks();
        this.AddInfrastructureServiceMocks();
    }

    public IServiceProvider Build() => this._services.BuildServiceProvider();

    private void AddFakeValidators() => this._services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    private void AddFakeMediatorRequests() => this._services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    });

    private void ReplaceServicesWithMocks() => this._services.ReplaceServiceWithMock<IEventSimulator>(ServiceLifetime.Singleton);

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

    private void AddInfrastructureServiceMocks()
    {
        var screenServiceMock = new Mock<IScreenService>();
        screenServiceMock.Setup(ss =>
                ss.IsMouseCursorInCorner(ScreenCorner.TopLeft, Testing.TopLeftCorner.X, Testing.TopLeftCorner.Y))
            .Returns(true);
        this._services.AddTransient(_ => screenServiceMock.Object);
    }
    
    private void AddInfrastructureWrapperMocks()
    {
        var processWrapperMock = new Mock<IProcessWrapper>();
        this._services.AddTransient(_ => processWrapperMock.Object);
    }
}

public static class ServiceCollectionExtensions
{
    public static void ReplaceServiceWithMock<TService>(this IServiceCollection services, ServiceLifetime serviceLifetime)
        where TService : class
    {
        var service = services.Single(sd => sd.ServiceType == typeof(TService));
        services.Remove(service);
        var replace = new Mock<TService>();
        var serviceDescriptor = new ServiceDescriptor(typeof(TService), _ => replace.Object, serviceLifetime);
        services.Add(serviceDescriptor);
    }
}
