namespace TcpClient.Extensions.ServiceCollectionExtensions;

using Microsoft.Extensions.DependencyInjection;
using TcpClient.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        ConfigureAppServices(services);

        return services;
    }

    private static IServiceCollection ConfigureAppServices(IServiceCollection services)
    {
        services.AddHostedService<SocketProcessorService>();

        services.AddSingleton<IConsoleInputProcessorService, ConsoleInputProcessorService>();
        services.AddSingleton<ISocketService, SocketService>();
        services.AddSingleton<ISocketReceiverService, SocketReceiverService>();
        services.AddSingleton<ISocketWriterService, SocketWriterService>();
        services.AddSingleton<IMessageHandlerService, MessageHandlerService>();

        return services;
    }
}
