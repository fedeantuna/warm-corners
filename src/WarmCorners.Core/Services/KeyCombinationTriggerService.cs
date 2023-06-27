using Microsoft.Extensions.Logging;
using SharpHook;
using WarmCorners.Core.Services.Abstractions;

namespace WarmCorners.Core.Services;

public class KeyCombinationTriggerService : IKeyCombinationTriggerService
{
    private readonly IEventSimulator _eventSimulator;
    private readonly ILogger<KeyCombinationTriggerService> _logger;
    private readonly IScreenService _screenService;

    public KeyCombinationTriggerService(IEventSimulator eventSimulator,
        ILogger<KeyCombinationTriggerService> logger,
        IScreenService screenService)
    {
        this._logger = logger;
        this._eventSimulator = eventSimulator;
        this._screenService = screenService;
    }

    public void ProcessKeyCombinationTrigger(IEnumerable<KeyCombinationTrigger> keyCombinationTriggers, int x, int y) =>
        keyCombinationTriggers.ToList().ForEach(keyCombination =>
        {
            var shouldTriggerCommand =
                this._screenService.IsMouseCursorInCorner(keyCombination.ScreenCorner, x, y);

            if (!shouldTriggerCommand || !keyCombination.KeyCodes.Any())
                return;

            keyCombination.KeyCodes.ToList().ForEach(key => this._eventSimulator.SimulateKeyPress(key));
            keyCombination.KeyCodes.ToList().ForEach(key => this._eventSimulator.SimulateKeyRelease(key));

            this._logger.LogInformation("Executed {KeyCombination}",
                string.Join('+', keyCombination.KeyCodes.Select(kc => kc.ToString())));
        });
}
