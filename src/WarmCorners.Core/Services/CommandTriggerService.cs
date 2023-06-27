using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Core.Wrappers;

namespace WarmCorners.Core.Services;

public class CommandTriggerService : ICommandTriggerService
{
    private readonly ILogger<CommandTriggerService> _logger;
    private readonly IScreenService _screenService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CommandTriggerService(ILogger<CommandTriggerService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IScreenService screenService)
    {
        this._logger = logger;
        this._serviceScopeFactory = serviceScopeFactory;
        this._screenService = screenService;
    }

    public void ProcessCommandTrigger(IEnumerable<CommandTrigger> commandTriggers, int x, int y) =>
        commandTriggers.ToList().ForEach(commandTrigger =>
        {
            var shouldTriggerCommand =
                this._screenService.IsMouseCursorInCorner(commandTrigger.ScreenCorner, x, y);

            if (!shouldTriggerCommand)
                return;

            using var scope = this._serviceScopeFactory.CreateScope();
            var processWrapper = scope.ServiceProvider.GetRequiredService<IProcessWrapper>();
            processWrapper.SetStartInfo(new ProcessStartInfo("cmd", $"/c {commandTrigger.Command}")
            {
                CreateNoWindow = true
            });
            var commandExecutedSuccessfully = processWrapper.Start();

            if (commandExecutedSuccessfully)
                this._logger.LogInformation("Executed {Command}", commandTrigger.Command);
            else
                this._logger.LogError("Could not run {Command}", commandTrigger.Command);
        });
}
