using SharpHook.Native;
using WarmCorners.Core.Common;

namespace WarmCorners.Core.Services.Abstractions;

public interface IKeyCombinationTriggerService
{
    void ProcessKeyCombinationTrigger(IEnumerable<KeyCombinationTrigger> keyCombinationTriggers, int x, int y);
}

public class KeyCombinationTrigger
{
    public required ScreenCorner ScreenCorner { get; init; }
    public required IReadOnlyCollection<KeyCode> KeyCodes { get; init; }
}