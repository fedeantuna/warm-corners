using System.Reactive.Linq;
using MediatR;
using Microsoft.Extensions.Options;
using SharpHook.Reactive;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Application.KeyCombinationTriggers.Commands;
using WarmCorners.Application.ShellTriggers.Commands;
using WarmCorners.Service.Configurations;
using WarmCorners.Service.Mappers;

namespace WarmCorners.Service.Workers;

public class MainWorker : BackgroundService
{
    private readonly IReactiveGlobalHook _reactiveGlobalHook;
    private readonly ISchedulerWrapper _schedulerWrapper;
    private readonly ISender _sender;
    private readonly IOptionsMonitor<TriggerConfiguration> _triggerConfigurationMonitor;

    public MainWorker(IOptionsMonitor<TriggerConfiguration> triggerConfigurationMonitor,
        IReactiveGlobalHook reactiveGlobalHook,
        ISchedulerWrapper schedulerWrapper,
        ISender sender)
    {
        this._triggerConfigurationMonitor = triggerConfigurationMonitor;
        this._reactiveGlobalHook = reactiveGlobalHook;
        this._schedulerWrapper = schedulerWrapper;
        this._sender = sender;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._reactiveGlobalHook.MouseMoved
            .Throttle(TimeSpan.FromMilliseconds(100), this._schedulerWrapper.Default)
            .Subscribe(args =>
            {
                var position = (args.Data.X, args.Data.Y);

                this.SendShellTriggers(position, stoppingToken);
                this.SendKeyCombinationTriggers(position, stoppingToken);
            }, stoppingToken);

        await this._reactiveGlobalHook.RunAsync();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        this._reactiveGlobalHook.Dispose();

        return base.StopAsync(cancellationToken);
    }

    private void SendShellTriggers((short x, short y) position, CancellationToken cancellationToken)
    {
        var shellTriggerConfigurations = this._triggerConfigurationMonitor
            .CurrentValue
            .CommandTriggers;

        foreach (var commandTriggerConfiguration in shellTriggerConfigurations)
        {
            var screenCorner = commandTriggerConfiguration.ScreenCorner.ToScreenCorner();

            if (!screenCorner.HasValue)
                break;

            var processShellTriggerCommand = new ProcessShellTriggerCommand
            {
                ScreenCorner = commandTriggerConfiguration.ScreenCorner.ToScreenCorner()!.Value,
                ShellCommand = commandTriggerConfiguration.ShellCommand,
                Position = position
            };

            this._sender.Send(processShellTriggerCommand, cancellationToken);
        }
    }

    private void SendKeyCombinationTriggers((short x, short y) position, CancellationToken cancellationToken)
    {
        var keyCombinationTriggerConfigurations = this._triggerConfigurationMonitor
            .CurrentValue
            .KeyCombinationTriggers;

        foreach (var keyCombinationTriggerConfiguration in keyCombinationTriggerConfigurations)
        {
            var screenCorner = keyCombinationTriggerConfiguration.ScreenCorner.ToScreenCorner();

            if (!screenCorner.HasValue)
                break;

            var keyCombination = keyCombinationTriggerConfiguration.KeyCombination.ToKeyCombinationKeyCodes();

            if (!keyCombination.Any())
                break;

            var processShellTriggerCommand = new ProcessKeyCombinationTriggerCommand
            {
                ScreenCorner = keyCombinationTriggerConfiguration.ScreenCorner.ToScreenCorner()!.Value,
                KeyCombination = keyCombination,
                Position = position
            };

            this._sender.Send(processShellTriggerCommand, cancellationToken);
        }
    }
}
