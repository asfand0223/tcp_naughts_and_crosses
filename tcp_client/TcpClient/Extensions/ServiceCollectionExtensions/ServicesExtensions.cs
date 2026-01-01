namespace TcpClient.Extensions.ServiceCollectionExtensions;

using Microsoft.Extensions.DependencyInjection;
using TcpClient.Services;
using TcpClient.Validators;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        ConfigureAppServices(services);
        ConfigureServiceValidators(services);

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
        services.AddSingleton<IGameService, GameService>();

        return services;
    }

    private static IServiceCollection ConfigureServiceValidators(IServiceCollection services)
    {
        services.AddSingleton<IGameServiceValidator, GameServiceValidator>();

        return services;
    }
}
