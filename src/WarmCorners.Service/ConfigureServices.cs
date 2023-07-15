using SharpHook.Reactive;
using WarmCorners.Service.Configurations;
using WarmCorners.Service.Workers;

namespace WarmCorners.Service;

public static class ConfigureServices
{
    public static void AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IReactiveGlobalHook, SimpleReactiveGlobalHook>();

        services.AddConfigurations(configuration);

        services.AddHostedService<MainWorker>();
    }

    private static void AddConfigurations(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<TriggerConfiguration>(configuration.GetSection("Triggers"));
}
