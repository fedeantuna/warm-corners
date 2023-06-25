using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Sinks.InMemory;
using WarmCorners.Service.Infrastructure.Wrapper;
using WarmCorners.Service.Wrappers;

namespace WarmCorners.Service.Tests.Unit;

public class ServiceProviderBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public ServiceProviderBuilder()
    {
        this._services.AddInfrastructureServices()
            .AddPresentationServices(new ConfigurationBuilder().Build());

        this.ReplaceWrappersWithMocks();

        this.SetupInMemoryLogger();
    }

    public IServiceProvider Build() => this._services.BuildServiceProvider();

    private void ReplaceWrappersWithMocks()
    {
        var simpleReactiveGlobalHookWrapper = this._services.Single(sd => sd.ServiceType == typeof(ISimpleReactiveGlobalHookWrapper));
        this._services.Remove(simpleReactiveGlobalHookWrapper);
        var simpleReactiveGlobalHookWrapperMock = new Mock<ISimpleReactiveGlobalHookWrapper>();
        this._services.AddSingleton(_ => simpleReactiveGlobalHookWrapperMock.Object);

        var user32Wrapper = this._services.Single(sd => sd.ServiceType == typeof(IUser32Wrapper));
        this._services.Remove(user32Wrapper);
        var user32WrapperMock = new Mock<IUser32Wrapper>();
        this._services.AddSingleton(_ => user32WrapperMock.Object);
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
