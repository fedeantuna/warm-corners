using System.Reflection;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using SharpHook;
using WarmCorners.Application.Common.Behaviors;

namespace WarmCorners.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.ConfigureValidators();
        services.ConfigureMediator();

        services.AddSingleton<IEventSimulator, EventSimulator>();

        return services;
    }

    private static void ConfigureValidators(this IServiceCollection services) =>
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    private static void ConfigureMediator(this IServiceCollection services) =>
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            cfg.AddRequestPreProcessor(typeof(IRequestPreProcessor<>), typeof(LoggingBehavior<>));

            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
}
