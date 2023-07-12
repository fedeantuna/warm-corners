using SharpHook.Native;

namespace WarmCorners.Service.Mappers;

public static class KeyCombinationMapper
{
    public static IReadOnlyCollection<KeyCode> ToKeyCombinationKeyCodes(this string keyCombination)
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
