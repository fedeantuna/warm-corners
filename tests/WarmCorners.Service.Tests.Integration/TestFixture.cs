namespace WarmCorners.Service.Tests.Integration;

[SetUpFixture]
public class TestFixture
{
    [OneTimeSetUp]
    public void RunBeforeAnyTest() =>
        Testing.StartTestApplication();
}
