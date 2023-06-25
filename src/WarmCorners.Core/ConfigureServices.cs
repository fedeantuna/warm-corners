using Microsoft.Extensions.DependencyInjection;
using WarmCorners.Core.Services;
using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Core.Wrappers;

namespace WarmCorners.Core;

public static class ConfigureServices
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<ICommandTriggerService, CommandTriggerService>();
        services.AddSingleton<IKeyCombinationTriggerService, KeyCombinationTriggerService>();

        services.AddCoreWrappers();

        return services;
    }

    private static void AddCoreWrappers(this IServiceCollection services)
    {
        services.AddSingleton<IEventSimulatorWrapper, EventSimulatorWrapper>();
        services.AddSingleton<IProcessWrapper, ProcessWrapper>();
    }
}
