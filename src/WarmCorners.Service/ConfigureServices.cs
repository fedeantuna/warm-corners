using SharpHook.Reactive;
using WarmCorners.Service.Configurations;
using WarmCorners.Service.Workers;

namespace WarmCorners.Service;

public static class ConfigureServices
{
    public static void AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IReactiveGlobalHook, SimpleReactiveGlobalHook>();

        services.Configure<TriggerConfiguration>(configuration.GetSection("Triggers"));

        services.AddHostedService<MainWorker>();
    }
}
