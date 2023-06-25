using Microsoft.Extensions.Logging;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Core.Wrappers;

namespace WarmCorners.Core.Services;

public class KeyCombinationTriggerService : IKeyCombinationTriggerService
{
    private readonly IEventSimulatorWrapper _eventSimulatorWrapper;
    private readonly ILogger<KeyCombinationTriggerService> _logger;
    private readonly IScreenService _screenService;

    public KeyCombinationTriggerService(IEventSimulatorWrapper eventSimulatorWrapper,
        ILogger<KeyCombinationTriggerService> logger,
        IScreenService screenService)
    {
        this._logger = logger;
        this._eventSimulatorWrapper = eventSimulatorWrapper;
        this._screenService = screenService;
    }

    public void ProcessKeyCombinationTrigger(IEnumerable<KeyCombinationTrigger> keyCombinationTriggers, int x, int y) =>
        keyCombinationTriggers.ToList().ForEach(keyCombination =>
        {
            var shouldTriggerCommand =
                this._screenService.IsMouseCursorInCorner(keyCombination.ScreenCorner, x, y);

            if (!shouldTriggerCommand || !keyCombination.KeyCodes.Any())
                return;

            keyCombination.KeyCodes.ToList().ForEach(key => this._eventSimulatorWrapper.SimulateKeyPress(key));
            keyCombination.KeyCodes.ToList().ForEach(key => this._eventSimulatorWrapper.SimulateKeyRelease(key));

            this._logger.LogInformation("Executed {KeyCombination}",
                string.Join('+', keyCombination.KeyCodes.Select(kc => kc.ToString())));
        });
}
