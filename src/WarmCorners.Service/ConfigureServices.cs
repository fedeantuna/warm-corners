using SharpHook.Reactive;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Service.Configurations;
using WarmCorners.Service.Infrastructure.Services;
using WarmCorners.Service.Infrastructure.Wrapper;
using WarmCorners.Service.Workers;
using WarmCorners.Service.Wrappers;

namespace WarmCorners.Service;

public static class ConfigureServices
{
    public static void AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IReactiveGlobalHook, SimpleReactiveGlobalHook>();

        services.AddPresentationWrappers();

        services.AddConfigurations(configuration);

        services.AddHostedService<MainWorker>();
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IScreenService, ScreenService>();

        services.AddInfrastructureWrappers();

        return services;
    }

    private static void AddConfigurations(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<TriggerConfiguration>(configuration.GetSection("Triggers"));

    private static void AddPresentationWrappers(this IServiceCollection services) =>
        services.AddSingleton<ISimpleReactiveGlobalHookWrapper, SimpleReactiveGlobalHookWrapper>();

    private static void AddInfrastructureWrappers(this IServiceCollection services) =>
        services.AddSingleton<IUser32Wrapper, User32Wrapper>();
}
