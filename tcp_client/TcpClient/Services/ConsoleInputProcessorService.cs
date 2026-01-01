namespace TcpClient.Services;

using System.Net.Sockets;
using Microsoft.Extensions.Hosting;

public interface IConsoleInputProcessorService
{
    public Task ProcessAsync(Socket socket, CancellationToken cancellationToken);
}

public class ConsoleInputProcessorService : IConsoleInputProcessorService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    private readonly ISocketService _socketService;
    private readonly ISocketWriterService _socketWriterService;

    public ConsoleInputProcessorService(
        IHostApplicationLifetime hostApplicationLifetime,
        ISocketService socketService,
        ISocketWriterService socketWriterService
    )
    {
        _hostApplicationLifetime = hostApplicationLifetime;

        _socketService = socketService;
        _socketWriterService = socketWriterService;
    }

    public async Task ProcessAsync(Socket socket, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Type to enter a message:");

            var message = Console.ReadLine();

            if (message is not null)
            {
                if (message.ToLower() == "quit")
                {
                    _hostApplicationLifetime.StopApplication();
                    break;
                }

                await _socketWriterService.WriteAsync(socket, message);
            }
        }
    }
}
