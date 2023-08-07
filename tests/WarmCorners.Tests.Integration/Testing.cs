using Microsoft.Reactive.Testing;

namespace WarmCorners.Tests.Integration;

public static class Testing
{
    public static (short Width, short Height) TestDisplaySize => (1024, 768);
    public static TestScheduler TestScheduler { get; } = new();
}
