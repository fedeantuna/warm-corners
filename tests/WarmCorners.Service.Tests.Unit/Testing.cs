namespace WarmCorners.Service.Tests.Unit;

public static class Testing
{
    public static (int Width, int Height) TestDisplaySize => (1024, 768);
    public static (int X, int Y) TopLeftCorner => (0, 0);
    public static (int X, int Y) TopRightCorner => (TestDisplaySize.Width, 0);
    public static (int X, int Y) BottomRightCorner => (TestDisplaySize.Width, TestDisplaySize.Height);
    public static (int X, int Y) BottomLeftCorner => (0, TestDisplaySize.Height);
}
