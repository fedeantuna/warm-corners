using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Sinks.InMemory;

namespace WarmCorners.Tests.Common;

public static class ServiceCollectionExtensions
{
    public static Mock<TService> ReplaceServiceWithMock<TService>(this IServiceCollection services, ServiceLifetime serviceLifetime)
        where TService : class
    {
        var service = services.Single(sd => sd.ServiceType == typeof(TService));
        services.Remove(service);
        var replace = new Mock<TService>();
        var serviceDescriptor = new ServiceDescriptor(typeof(TService), _ => replace.Object, serviceLifetime);
        services.Add(serviceDescriptor);

        return replace;
    }

    public static void SetupInMemoryLogger(this IServiceCollection services) =>
        services.AddLogging(builder =>
        {
            builder.ClearProviders();

            var logger = new LoggerConfiguration()
                .WriteTo.InMemory()
                .MinimumLevel.Verbose()
                .CreateLogger();
            builder.AddSerilog(logger);
        });
}
