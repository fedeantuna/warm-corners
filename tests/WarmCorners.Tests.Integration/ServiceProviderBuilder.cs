using Microsoft.Extensions.DependencyInjection;
using SharpHook;
using WarmCorners.Application;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Infrastructure;
using WarmCorners.Infrastructure.Wrappers;
using WarmCorners.Tests.Common;

namespace WarmCorners.Tests.Integration;

public class ServiceProviderBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public ServiceProviderBuilder()
    {
        this._services.AddApplicationServices()
            .AddInfrastructureServices()
            .SetupInMemoryLogger();

        this.ReplaceServicesWithMocks();
        this.ReplaceWrappersWithMocks();
    }

    public IServiceProvider Build() => this._services.BuildServiceProvider();

    private void ReplaceServicesWithMocks() =>
        this._services.ReplaceServiceWithMock<IEventSimulator>(ServiceLifetime.Singleton);

    private void ReplaceWrappersWithMocks()
    {
        this._services.ReplaceServiceWithMock<IDateTimeOffsetWrapper>(ServiceLifetime.Transient);
        this._services.ReplaceServiceWithMock<IProcessWrapper>(ServiceLifetime.Transient);

        var schedulerWrapperMock = this._services.ReplaceServiceWithMock<ISchedulerWrapper>(ServiceLifetime.Transient);
        schedulerWrapperMock.SetupGet(sw => sw.Default).Returns(Testing.TestScheduler);

        var user32WrapperMock = this._services.ReplaceServiceWithMock<IUser32Wrapper>(ServiceLifetime.Transient);
        user32WrapperMock.Setup(u32W =>
            u32W.GetScreenResolution()).Returns((Testing.TestDisplaySize.Width, Testing.TestDisplaySize.Height));
    }
}
