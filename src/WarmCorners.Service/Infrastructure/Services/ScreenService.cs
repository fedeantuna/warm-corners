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
        var (thresholdX, thresholdY) = this.GetScreenCornerThresholds(screenCorner);

        return screenCorner switch
        {
            ScreenCorner.TopLeft => x <= thresholdX && y <= thresholdY,
            ScreenCorner.TopRight => x >= thresholdX && y <= thresholdY,
            ScreenCorner.BottomRight => x >= thresholdX && y >= thresholdY,
            ScreenCorner.BottomLeft => x <= thresholdX && y >= thresholdY,
            _ => throw new ArgumentOutOfRangeException(nameof(screenCorner), screenCorner, null)
        };
    }

    private (int ThresholdX, int ThresholdY) GetScreenCornerThresholds(ScreenCorner screenCorner)
    {
        var (width, height) = this._user32Wrapper.GetScreenResolution();

        return screenCorner switch
        {
            ScreenCorner.TopLeft => (0, 0),
            ScreenCorner.TopRight => (width, 0),
            ScreenCorner.BottomRight => (width, height),
            ScreenCorner.BottomLeft => (0, height),
            _ => throw new ArgumentOutOfRangeException(nameof(screenCorner), screenCorner, null)
        };
    }
}
