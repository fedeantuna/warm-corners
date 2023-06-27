using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using SharpHook.Reactive;
using WarmCorners.Service.Workers;

namespace WarmCorners.Service.Tests.Unit.Workers;

public class MainWorkerTests
{
    private readonly IHostedService _mainWorker;
    private readonly Mock<IReactiveGlobalHook> _reactiveGlobalHookMock;

    public MainWorkerTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        var reactiveGlobalHook = provider.GetRequiredService<IReactiveGlobalHook>();
        this._reactiveGlobalHookMock = Mock.Get(reactiveGlobalHook);

        this._mainWorker = provider.GetServices<IHostedService>().Single(hs => hs.GetType() == typeof(MainWorker));
    }

    [Fact]
    public void StopAsync_DisposesReactiveGlobalHook()
    {
        // Act
        this._mainWorker.StopAsync(default);

        // Assert
        this._reactiveGlobalHookMock.Verify(rgh =>
            rgh.Dispose(), Times.Once);
    }
}
