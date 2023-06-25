using WarmCorners.Core;
using WarmCorners.Service;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, builder) => builder.AddLogging(context.HostingEnvironment))
    .ConfigureServices(services => services.AddCoreServices()
        .AddInfrastructureServices()
        .AddPresentationServices())
    .Build();

host.Run();
