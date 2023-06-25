using WarmCorners.Core.Common;

namespace WarmCorners.Core.Services.Abstractions;

public interface IScreenService
{
    bool IsMouseCursorInCorner(ScreenCorner screenCorner, int x, int y);
}
