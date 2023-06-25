using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Sinks.InMemory;
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

        this.AddPresentationServiceMocks();

        this.SetupInMemoryLogger();
    }

    public IServiceProvider Build() => this._services.BuildServiceProvider();

    private void ReplaceWrappersWithMocks()
    {
        var eventSimulatorWrapper = this._services.Single(sd => sd.ServiceType == typeof(IEventSimulatorWrapper));
        this._services.Remove(eventSimulatorWrapper);
        var eventSimulatorWrapperMock = new Mock<IEventSimulatorWrapper>();
        this._services.AddSingleton(_ => eventSimulatorWrapperMock.Object);

        var processWrapper = this._services.Single(sd => sd.ServiceType == typeof(IProcessWrapper));
        this._services.Remove(processWrapper);
        var processWrapperMock = new Mock<IProcessWrapper>();
        this._services.AddSingleton(_ => processWrapperMock.Object);
    }

    private void AddPresentationServiceMocks()
    {
        var screenServiceMock = new Mock<IScreenService>();
        this._services.AddSingleton(_ => screenServiceMock.Object);
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
