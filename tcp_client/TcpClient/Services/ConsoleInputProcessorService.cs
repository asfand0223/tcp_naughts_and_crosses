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

            var message = await ReadLineAsnc(cancellationToken);

            if (message is null)
            {
                continue;
            }

            if (message.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                _hostApplicationLifetime.StopApplication();
                break;
            }

            await _socketWriterService.WriteAsync(socket, message);
        }
    }

    public async Task<string?> ReadLineAsnc(CancellationToken cancellationToken)
    {
        var readTask = Task.Run(() => Console.ReadLine(), cancellationToken);

        var completed = await Task.WhenAny(
            readTask,
            Task.Delay(Timeout.Infinite, cancellationToken)
        );

        if (completed == readTask)
        {
            return readTask.Result;
        }

        return null;
    }
}
