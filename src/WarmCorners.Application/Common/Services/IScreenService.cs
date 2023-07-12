using WarmCorners.Domain.Enums;

namespace WarmCorners.Application.Common.Services;

public interface IScreenService
{
    bool IsMouseCursorInCorner(ScreenCorner screenCorner, short x, short y);
}
