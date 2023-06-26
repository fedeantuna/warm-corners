using SharpHook.Native;
using WarmCorners.Core.Common;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Service.Configurations;

namespace WarmCorners.Service.Mappers;

public static class KeyCombinationTriggerMapper
{
    public static IEnumerable<KeyCombinationTrigger> ToKeyCombinationTriggers(
        this List<KeyCombinationTriggerConfiguration> keyCombinationTriggerConfigurations)
    {
        foreach (var keyCombinationTriggerConfiguration in keyCombinationTriggerConfigurations)
            if (Enum.TryParse(typeof(ScreenCorner), keyCombinationTriggerConfiguration.ScreenCorner, true, out var screenCorner))
            {
                var keyCodes = keyCombinationTriggerConfiguration.KeyCombination.ToKeyCodes();

                if (keyCodes.Any())
                    yield return new KeyCombinationTrigger
                    {
                        ScreenCorner = (ScreenCorner)screenCorner,
                        KeyCodes = keyCodes
                    };
            }
    }

    private static IReadOnlyCollection<KeyCode> ToKeyCodes(this string keyCombination)
    {
        var keys = keyCombination.Split('+').ToList();

        var keyCodes = new List<KeyCode>();
        foreach (var key in keys)
            if (Enum.TryParse(typeof(KeyCode), $"Vc{key}", true, out var keyCode))
            {
                keyCodes.Add((KeyCode)keyCode);
            }
            else
            {
                keyCodes.Clear();
                break;
            }

        return keyCodes;
    }
}
