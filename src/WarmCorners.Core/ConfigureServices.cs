using Microsoft.Extensions.DependencyInjection;
using SharpHook;
using WarmCorners.Core.Services;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Core.Wrappers;

namespace WarmCorners.Core;

public static class ConfigureServices
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<ICommandTriggerService, CommandTriggerService>();
        services.AddSingleton<IEventSimulator, EventSimulator>();
        services.AddSingleton<IKeyCombinationTriggerService, KeyCombinationTriggerService>();

        services.AddCoreWrappers();

        return services;
    }

    private static void AddCoreWrappers(this IServiceCollection services)
    {
        services.AddSingleton<IProcessWrapper, ProcessWrapper>();
        services.AddSingleton<ISchedulerWrapper, SchedulerWrapper>();
    }
}
