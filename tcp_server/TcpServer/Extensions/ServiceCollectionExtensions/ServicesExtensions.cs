namespace TcpServer.Extensions.ServiceCollectionExtensions;

using Microsoft.Extensions.DependencyInjection;
using TcpServer.Services;

public static class ServicesExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        ConfigureAppHostedServices(services);
        ConfigureAppServices(services);

        return services;
    }

    private static IServiceCollection ConfigureAppHostedServices(IServiceCollection services)
    {
        services.AddHostedService<ListenSocketProcessorService>();

        return services;
    }

    private static IServiceCollection ConfigureAppServices(IServiceCollection services)
    {
        services.AddSingleton<ISocketReceiverService, SocketReceiveService>();
        services.AddSingleton<IListenSocketService, ListenSocketService>();
        services.AddSingleton<IMessageHandlerService, MessageHandlerService>();
        services.AddSingleton<IGamesService, GamesService>();
        services.AddSingleton<ISocketWriterService, SocketWriterService>();

        return services;
    }
}
