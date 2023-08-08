using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using SharpHook.Reactive;
using WarmCorners.Application.Common.Services;
using WarmCorners.Application.KeyCombinationTriggers.Commands.TriggerKeyCombination;
using WarmCorners.Application.ShellTriggers.Commands.TriggerShell;
using WarmCorners.Domain.Enums;
using WarmCorners.Service.Workers;

namespace WarmCorners.Service.Tests.Unit.Workers;

public class MainWorkerTests
{
    private readonly IHostedService _mainWorker;
    private readonly Mock<IReactiveGlobalHook> _reactiveGlobalHookMock;
    private readonly Mock<IScreenService> _screenServiceMock;
    private readonly Mock<ISender> _senderMock;

    public MainWorkerTests()
    {
        var provider = new ServiceProviderBuilder().Build();

        var reactiveGlobalHook = provider.GetRequiredService<IReactiveGlobalHook>();
        this._reactiveGlobalHookMock = Mock.Get(reactiveGlobalHook);
        var screenService = provider.GetRequiredService<IScreenService>();
        this._screenServiceMock = Mock.Get(screenService);
        var sender = provider.GetRequiredService<ISender>();
        this._senderMock = Mock.Get(sender);

        this._mainWorker = provider.GetServices<IHostedService>().Single(hs => hs.GetType() == typeof(MainWorker));
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotExecuteKeyCombinationWhenMouseCursorIsNotInCorrectCorner()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        this.SetupScreenServiceMockMouseCursorIsInCorner(false);

        await this._mainWorker.StartAsync(cancellationTokenSource.Token);

        // Act
        Testing.TriggerMouseEvent();

        // Assert
        this._senderMock.VerifyCommandIsNotCalled<TriggerKeyCombinationCommand>();

        cancellationTokenSource.Cancel();
    }

    [Fact]
    public async Task ExecuteAsync_DoesExecuteKeyCombinationWhenMouseCursorIsInCorrectCorner()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        this.SetupScreenServiceMockMouseCursorIsInCorner(true);

        await this._mainWorker.StartAsync(cancellationTokenSource.Token);

        // Act
        Testing.TriggerMouseEvent();

        // Assert
        this._senderMock.VerifyCommandIsCalledOnce<TriggerKeyCombinationCommand>();

        cancellationTokenSource.Cancel();
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotExecuteShellCommandWhenMouseCursorIsNotInCorrectCorner()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        this.SetupScreenServiceMockMouseCursorIsInCorner(false);

        await this._mainWorker.StartAsync(cancellationTokenSource.Token);

        // Act
        Testing.TriggerMouseEvent();

        // Assert
        this._senderMock.VerifyCommandIsNotCalled<TriggerShellCommand>();

        cancellationTokenSource.Cancel();
    }

    [Fact]
    public async Task ExecuteAsync_DoesExecuteShellCommandWhenMouseCursorIsInCorrectCorner()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        this.SetupScreenServiceMockMouseCursorIsInCorner(true);

        await this._mainWorker.StartAsync(cancellationTokenSource.Token);

        // Act
        Testing.TriggerMouseEvent();

        // Assert
        this._senderMock.VerifyCommandIsCalledOnce<TriggerShellCommand>();

        cancellationTokenSource.Cancel();
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

    private void SetupScreenServiceMockMouseCursorIsInCorner(bool mouseIsInCursor) =>
        this._screenServiceMock
            .Setup(ss => ss.IsMouseCursorInCorner(It.IsAny<ScreenCorner>(), It.IsAny<short>(), It.IsAny<short>()))
            .Returns(mouseIsInCursor);
}
