using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using SharpHook;
using SharpHook.Reactive;
using WarmCorners.Service;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, builder) =>
    {
        builder.ClearProviders();

        builder.AddSerilog(CreateLogger(context.HostingEnvironment));
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<IReactiveGlobalHook, SimpleReactiveGlobalHook>();
        services.AddSingleton<IEventSimulator, EventSimulator>();

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();

Logger CreateLogger(IHostEnvironment hostEnvironment)
{
    var loggerConfiguration = new LoggerConfiguration()
        .Enrich.FromLogContext();

    if (hostEnvironment.IsDevelopment())
        loggerConfiguration.MinimumLevel.Information();

    var logger = loggerConfiguration
        .WriteTo.Console(theme: AnsiConsoleTheme.Code)
        .WriteTo.File(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), rollingInterval: RollingInterval.Month)
        .CreateLogger();

    return logger;
}
