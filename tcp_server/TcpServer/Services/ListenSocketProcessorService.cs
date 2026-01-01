namespace TcpServer.Services;

using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ListenSocketProcessorService : BackgroundService
{
    private readonly ILogger<ListenSocketProcessorService> _logger;
    private readonly IListenSocketService _listenSocketService;

    public ListenSocketProcessorService(
        ILogger<ListenSocketProcessorService> logger,
        IListenSocketService socketService
    )
    {
        _logger = logger;
        _listenSocketService = socketService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            _listenSocketService.Listen(5000);

            await _listenSocketService.AcceptClientsAsync(cancellationToken);
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
        {
            _logger.LogInformation("Shutting down...");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _listenSocketService.Stop();

        await base.StopAsync(cancellationToken);
    }
}
