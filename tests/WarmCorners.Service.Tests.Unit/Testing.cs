namespace WarmCorners.Service.Tests.Unit;

public static class Testing
{
    public static (int Width, int Height) TestDisplaySize => (1024, 768);
    public static (int X, int Y) TopLeftCorner => (0, 0);
    public static (int X, int Y) TopRightCorner => (TestDisplaySize.Width, 0);
    public static (int X, int Y) BottomRightCorner => (TestDisplaySize.Width, TestDisplaySize.Height);
    public static (int X, int Y) BottomLeftCorner => (0, TestDisplaySize.Height);
    public static string SomeCommand => "run something";
    public static string SomeOtherCommand => "run some other thing";
    public static string SomeKeyCombination => "leftMeta+tab";
    public static string SomeOtherKeyCombination => "rightControl+RIGHTAlt+deleTE";
    public static string SomeInvalidKeyCombination => "rightControl+RIGHTAlt+deleteThis";
}
