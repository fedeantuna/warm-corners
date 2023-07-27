using Microsoft.Extensions.DependencyInjection;
using WarmCorners.Application.Common.Services;
using WarmCorners.Application.Common.Wrappers;
using WarmCorners.Infrastructure.Services;
using WarmCorners.Infrastructure.Wrappers;

namespace WarmCorners.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IScreenService, ScreenService>();

        services.AddWrappers();

        return services;
    }

    private static void AddWrappers(this IServiceCollection services)
    {
        services.AddTransient<IDateTimeOffsetWrapper, DateTimeOffsetWrapper>();
        services.AddTransient<IProcessWrapper, ProcessWrapper>();
        services.AddTransient<ISchedulerWrapper, SchedulerWrapper>();
        services.AddTransient<IUser32Wrapper, User32Wrapper>();
    }
}
