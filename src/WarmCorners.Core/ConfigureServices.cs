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

        services.AddSingleton<IProcessWrapper, ProcessWrapper>();

        return services;
    }
}
