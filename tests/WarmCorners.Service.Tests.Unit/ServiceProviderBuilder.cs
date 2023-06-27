using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Sinks.InMemory;
using SharpHook.Reactive;
using WarmCorners.Service.Infrastructure.Wrapper;

namespace WarmCorners.Service.Tests.Unit;

public class ServiceProviderBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public ServiceProviderBuilder()
    {
        this._services.AddInfrastructureServices()
            .AddPresentationServices(new ConfigurationBuilder().Build());

        this.ReplaceWrappersWithMocks();

        this.SetupReactiveGlobalHookMock();
        this.SetupInMemoryLogger();
    }

    public IServiceProvider Build() => this._services.BuildServiceProvider();

    private void ReplaceWrappersWithMocks()
    {
        var user32WrapperMock = this._services.ReplaceServiceWithMock<IUser32Wrapper>();
        user32WrapperMock.Setup(u32W =>
            u32W.GetScreenResolution()).Returns((Testing.TestDisplaySize.Width, Testing.TestDisplaySize.Height));
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
