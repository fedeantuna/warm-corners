using WarmCorners.Core.Common;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Service.Infrastructure.Wrapper;

namespace WarmCorners.Service.Infrastructure.Services;

public class ScreenService : IScreenService
{
    private readonly IUser32Wrapper _user32Wrapper;

    public ScreenService(IUser32Wrapper user32Wrapper) =>
        this._user32Wrapper = user32Wrapper;

    public bool IsMouseCursorInCorner(ScreenCorner screenCorner, int x, int y)
    {
        var (width, height) = this._user32Wrapper.GetScreenResolution();

        return screenCorner switch
        {
            ScreenCorner.TopLeft => x <= 0 && y <= 0,
            ScreenCorner.TopRight => x >= width && y <= 0,
            ScreenCorner.BottomRight => x >= width && y >= height,
            ScreenCorner.BottomLeft => x <= 0 && y >= height,
            _ => throw new ArgumentOutOfRangeException(nameof(screenCorner), screenCorner, null)
        };
    }
}
