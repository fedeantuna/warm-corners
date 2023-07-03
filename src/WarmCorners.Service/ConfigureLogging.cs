using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace WarmCorners.Service;

public static class ConfigureLogging
{
    public static void AddLogging(this ILoggingBuilder builder, IHostEnvironment context)
    {
        builder.ClearProviders();

        builder.AddSerilog(CreateLogger(context));
    }

    private static Logger CreateLogger(IHostEnvironment context)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext();

        loggerConfiguration.MinimumLevel.Information();
        if (context.IsDevelopment())
            loggerConfiguration.MinimumLevel.Debug();

        var logFile = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WarmCorners", "wc_.log");
        var logger = loggerConfiguration
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .WriteTo.File(logFile, rollingInterval: RollingInterval.Month)
            .CreateLogger();

        return logger;
    }
}
