namespace TcpServer.Extensions.ServiceCollectionExtensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class LoggingExtensions
{
    public static IServiceCollection ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging(opts =>
        {
            opts.ClearProviders();
            opts.AddConsole();
            opts.SetMinimumLevel(LogLevel.Information);
        });

        return services;
    }
}
