using WarmCorners.Core.Services.Abstractions;
using WarmCorners.Service.Infrastructure.Services;
using WarmCorners.Service.Infrastructure.Wrapper;

namespace WarmCorners.Service;

public static class ConfigureServices
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IScreenService, ScreenService>();

        services.AddSingleton<IUser32Wrapper, User32Wrapper>();
    }
}
