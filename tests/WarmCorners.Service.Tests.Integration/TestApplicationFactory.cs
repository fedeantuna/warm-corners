using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SharpHook;
using SharpHook.Reactive;
using WarmCorners.Core.Wrappers;
using WarmCorners.Service.Infrastructure.Wrapper;

namespace WarmCorners.Service.Tests.Integration;

internal class TestApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

                configurationBuilder.AddConfiguration(configuration);
            })
            .ConfigureServices(services =>
            {
                services.ReplaceServiceWithMock<IEventSimulator>();
                var reactiveGlobalHookMock = services.ReplaceServiceWithMock<IReactiveGlobalHook>();
                
                services.ReplaceServiceWithMock<IProcessWrapper>();
                var schedulerWrapperMock = services.ReplaceServiceWithMock<ISchedulerWrapper>();
                var user32WrapperMock = services.ReplaceServiceWithMock<IUser32Wrapper>();

                reactiveGlobalHookMock
                    .Setup(rgh => rgh.MouseMoved)
                    .Returns(Testing.TestMouseMoved);
                reactiveGlobalHookMock
                    .Setup(rgh => rgh.RunAsync())
                    .Returns(new Subject<Unit>().AsObservable());
                reactiveGlobalHookMock
                    .Setup(rgh => rgh.Dispose())
                    .Callback(() => { });
                
                schedulerWrapperMock
                    .Setup(sw => sw.Default)
                    .Returns(Testing.TestScheduler);

                user32WrapperMock
                    .Setup(u32 => u32.GetScreenResolution())
                    .Returns(Testing.TestDisplaySize);
            })
            .Configure(_ => { });
}

public static class ServiceCollectionExtensions
{
    public static Mock<TIService> ReplaceServiceWithMock<TIService>(this IServiceCollection services)
        where TIService : class
    {
        var service = services.Single(sd => sd.ServiceType == typeof(TIService));
        services.Remove(service);
        var replace = new Mock<TIService>();
        services.AddSingleton(_ => replace.Object);

        return replace;
    }
}
