using System.Reactive.Linq;
using Microsoft.Extensions.Options;
using SharpHook.Reactive;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Core.Wrappers;
using WarmCorners.Service.Configurations;
using WarmCorners.Service.Mappers;

namespace WarmCorners.Service.Workers;

public class MainWorker : BackgroundService
{
    private readonly ICommandTriggerService _commandTriggerService;
    private readonly IKeyCombinationTriggerService _keyCombinationTriggerService;
    private readonly IReactiveGlobalHook _reactiveGlobalHook;
    private readonly ISchedulerWrapper _schedulerWrapper;
    private readonly IOptionsMonitor<TriggerConfiguration> _triggerConfigurationMonitor;

    public MainWorker(ICommandTriggerService commandTriggerService,
        IKeyCombinationTriggerService keyCombinationService,
        IOptionsMonitor<TriggerConfiguration> triggerConfigurationMonitor,
        IReactiveGlobalHook reactiveGlobalHook,
        ISchedulerWrapper schedulerWrapper)
    {
        this._commandTriggerService = commandTriggerService;
        this._keyCombinationTriggerService = keyCombinationService;
        this._triggerConfigurationMonitor = triggerConfigurationMonitor;
        this._reactiveGlobalHook = reactiveGlobalHook;
        this._schedulerWrapper = schedulerWrapper;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._reactiveGlobalHook.MouseMoved
            .Throttle(TimeSpan.FromMilliseconds(100), this._schedulerWrapper.Default)
            .Subscribe(args =>
            {
                var (x, y) = (args.Data.X, args.Data.Y);

                var commandTriggers = this._triggerConfigurationMonitor
                    .CurrentValue
                    .CommandTriggers
                    .ToCommandTriggers();
                this._commandTriggerService.ProcessCommandTrigger(commandTriggers, x, y);

                var keyCombinationTriggers = this._triggerConfigurationMonitor
                    .CurrentValue
                    .KeyCombinationTriggers
                    .ToKeyCombinationTriggers();
                this._keyCombinationTriggerService.ProcessKeyCombinationTrigger(keyCombinationTriggers, x, y);
            }, stoppingToken);

        await this._reactiveGlobalHook.RunAsync();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        this._reactiveGlobalHook.Dispose();

        return base.StopAsync(cancellationToken);
    }
}
