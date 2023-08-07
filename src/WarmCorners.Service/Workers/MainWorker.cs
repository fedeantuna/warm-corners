using System.Reactive.Linq;
using MediatR;
using Microsoft.Extensions.Options;
using SharpHook.Reactive;
using WarmCorners.Application.Common.Services;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Application.KeyCombinationTriggers.Commands.TriggerKeyCombination;
using WarmCorners.Application.ShellTriggers.Commands.TriggerShell;
using WarmCorners.Domain.Enums;
using WarmCorners.Service.Configurations;
using WarmCorners.Service.Mappers;

namespace WarmCorners.Service.Workers;

public class MainWorker : BackgroundService
{
    private readonly IReactiveGlobalHook _reactiveGlobalHook;
    private readonly IScreenService _screenService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IOptionsMonitor<TriggerConfiguration> _triggerConfigurationMonitor;

    public MainWorker(IOptionsMonitor<TriggerConfiguration> triggerConfigurationMonitor,
        IReactiveGlobalHook reactiveGlobalHook,
        IScreenService screenService,
        IServiceScopeFactory serviceScopeFactory)
    {
        this._triggerConfigurationMonitor = triggerConfigurationMonitor;
        this._reactiveGlobalHook = reactiveGlobalHook;
        this._screenService = screenService;
        this._serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = this._serviceScopeFactory.CreateScope();
        var schedulerWrapper = scope.ServiceProvider.GetRequiredService<ISchedulerWrapper>();
        this._reactiveGlobalHook.MouseMoved
            .Throttle(TimeSpan.FromMilliseconds(100), schedulerWrapper.Default)
            .Subscribe(args =>
            {
                var position = (args.Data.X, args.Data.Y);

                this.ProcessShellTriggers(position, stoppingToken);
                this.ProcessKeyCombinationTriggers(position, stoppingToken);
            }, stoppingToken);

        await this._reactiveGlobalHook.RunAsync();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        this._reactiveGlobalHook.Dispose();

        return base.StopAsync(cancellationToken);
    }

    private void ProcessShellTriggers((short X, short Y) position, CancellationToken cancellationToken) =>
        this._triggerConfigurationMonitor
            .CurrentValue
            .ShellTriggers
            .ForEach(st =>
            {
                var triggerShellCommand = new TriggerShellCommand
                {
                    ShellCommand = st.ShellCommand
                };
                var screenCorner = st.ScreenCorner.ToScreenCorner();

                this.ProcessCommand(triggerShellCommand,
                    screenCorner,
                    position,
                    cancellationToken);
            });

    private void ProcessKeyCombinationTriggers((short X, short Y) position, CancellationToken cancellationToken) =>
        this._triggerConfigurationMonitor
            .CurrentValue
            .KeyCombinationTriggers
            .ForEach(kct =>
            {
                var keyCombination = kct.KeyCombination.ToKeyCombinationKeyCodes();
                var triggerKeyCombinationCommand = new TriggerKeyCombinationCommand
                {
                    KeyCombination = keyCombination
                };
                var screenCorner = kct.ScreenCorner.ToScreenCorner();

                this.ProcessCommand(triggerKeyCombinationCommand,
                    screenCorner,
                    position,
                    cancellationToken);
            });

    private void ProcessCommand(IBaseRequest command, ScreenCorner? screenCorner, (short X, short Y) position,
        CancellationToken cancellationToken)
    {
        var shouldTriggerCommand = screenCorner.HasValue
                                   && this._screenService.IsMouseCursorInCorner(screenCorner.Value, position.X, position.Y);

        using var scope = this._serviceScopeFactory.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        if (shouldTriggerCommand)
            sender.Send(command, cancellationToken);
    }
}
