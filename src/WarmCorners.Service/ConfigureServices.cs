using SharpHook.Reactive;
using WarmCorners.Service.Configurations;
using WarmCorners.Service.Infrastructure.Wrapper;
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

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddInfrastructureWrappers();

        return services;
    }

    private static void AddConfigurations(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<TriggerConfiguration>(configuration.GetSection("Triggers"));

    private static void AddInfrastructureWrappers(this IServiceCollection services) =>
        services.AddSingleton<IUser32Wrapper, User32Wrapper>();
}
