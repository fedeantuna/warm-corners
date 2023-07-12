using WarmCorners.Domain.Enums;

namespace WarmCorners.Service.Mappers;

public static class ScreenCornerMapper
{
    public static ScreenCorner? ToScreenCorner(this string screenCornerString)
    {
        if (Enum.TryParse(typeof(ScreenCorner), screenCornerString, true, out var screenCorner))
            return (ScreenCorner)screenCorner;

        return null;
    }
}
