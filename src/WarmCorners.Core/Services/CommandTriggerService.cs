using Microsoft.Extensions.Logging;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Core.Wrappers;

namespace WarmCorners.Core.Services;

public class CommandTriggerService : ICommandTriggerService
{
    private readonly ILogger<CommandTriggerService> _logger;
    private readonly IProcessWrapper _processWrapper;
    private readonly IScreenService _screenService;

    public CommandTriggerService(ILogger<CommandTriggerService> logger,
        IProcessWrapper processWrapper,
        IScreenService screenService)
    {
        this._logger = logger;
        this._processWrapper = processWrapper;
        this._screenService = screenService;
    }

    public void ProcessCommandTrigger(IEnumerable<CommandTrigger> commandTriggers, int x, int y) =>
        commandTriggers.ToList().ForEach(commandTrigger =>
        {
            var shouldTriggerCommand =
                this._screenService.IsMouseCursorInCorner(commandTrigger.ScreenCorner, x, y);

            if (!shouldTriggerCommand)
                return;

            var commandExecutedSuccessfully = this._processWrapper
                .Start("cmd", $"/c {commandTrigger.Command}");

            if (commandExecutedSuccessfully)
                this._logger.LogInformation("Executed {Command}", commandTrigger.Command);
            else
                this._logger.LogError("Could not run {Command}", commandTrigger.Command);
        });
}
