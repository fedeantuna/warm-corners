using WarmCorners.Core;
using WarmCorners.Service;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, builder) => builder.AddLogging(context.HostingEnvironment))
    .ConfigureServices((context, services) => services.AddCoreServices()
        .AddInfrastructureServices()
        .AddPresentationServices(context.Configuration))
    .Build();

await host.RunAsync();
