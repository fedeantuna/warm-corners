namespace WarmCorners.Core.Tests.Unit;

public static class Testing
{
    public static (int Width, int Height) TestDisplaySize => (1024, 768);
    public static (int X, int Y) TopLeftCorner => (0, 0);
    public static string CommandThatRunsSuccessfully => "run something successfully";
    public static string CommandThatRunsUnsuccessfully => "rum something unsuccessfully";
}
