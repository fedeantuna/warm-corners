using WarmCorners.Application;
using WarmCorners.Infrastructure;
using WarmCorners.Service;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, builder) => builder.AddLogging(context.HostingEnvironment))
    .ConfigureServices((context, services) => services.AddApplicationServices()
        .AddInfrastructureServices()
        .AddPresentationServices(context.Configuration))
    .Build();

await host.RunAsync();
