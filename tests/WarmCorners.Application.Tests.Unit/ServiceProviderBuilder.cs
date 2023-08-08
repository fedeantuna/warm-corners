using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SharpHook;
using WarmCorners.Application.Common.Services;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Domain.Enums;
using WarmCorners.Tests.Common;

namespace WarmCorners.Application.Tests.Unit;

public class ServiceProviderBuilder
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public ServiceProviderBuilder()
    {
        this._services.AddApplicationServices()
            .SetupInMemoryLogger();

        this.AddFakeValidators();
        this.AddFakeMediatorRequests();

        this.ReplaceServicesWithMocks();

        this.AddInfrastructureServiceMocks();
        this.AddInfrastructureWrapperMocks();
    }

    public IServiceProvider Build() => this._services.BuildServiceProvider();

    private void AddFakeValidators() => this._services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    private void AddFakeMediatorRequests() => this._services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    });

    private void ReplaceServicesWithMocks() => this._services.ReplaceServiceWithMock<IEventSimulator>(ServiceLifetime.Singleton);

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
