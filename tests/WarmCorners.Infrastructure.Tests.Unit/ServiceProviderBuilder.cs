using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Sinks.InMemory;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Infrastructure.Wrappers;
using WarmCorners.Tests.Common;

namespace WarmCorners.Infrastructure.Tests.Unit;

public class ServiceProviderBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public ServiceProviderBuilder()
    {
        this._services.AddInfrastructureServices()
            .SetupInMemoryLogger();

        this.ReplaceWrappersWithMocks();
    }

    public IServiceProvider Build() => this._services.BuildServiceProvider();

    private void ReplaceWrappersWithMocks()
    {
        this._services.ReplaceServiceWithMock<IDateTimeOffsetWrapper>(ServiceLifetime.Transient);
        this._services.ReplaceServiceWithMock<IProcessWrapper>(ServiceLifetime.Transient);
        this._services.ReplaceServiceWithMock<ISchedulerWrapper>(ServiceLifetime.Transient);

        var user32WrapperMock = this._services.ReplaceServiceWithMock<IUser32Wrapper>(ServiceLifetime.Transient);
        user32WrapperMock.Setup(u32W =>
            u32W.GetScreenResolution()).Returns((Testing.TestDisplaySize.Width, Testing.TestDisplaySize.Height));
    }
}
