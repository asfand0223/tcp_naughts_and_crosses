namespace TcpServer.Extensions.HostExtensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public static class ExceptionExtensions
{
    public static IHost UseGlobalExceptionHandling(this IHost host)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogCritical(e.ExceptionObject as Exception, "Unhandled exception occurred!");
        };

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogCritical(e.Exception, "Unobserved task exception!");
            e.SetObserved();
        };

        return host;
    }
}
