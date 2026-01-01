namespace TcpClient.Services;

using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SocketProcessorService : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    private readonly ILogger<SocketProcessorService> _logger;

    private readonly IConsoleInputProcessorService _consoleInputProcessService;
    private readonly ISocketService _socketService;
    private readonly ISocketReceiverService _socketReceiverService;

    public SocketProcessorService(
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<SocketProcessorService> logger,
        IConsoleInputProcessorService consoleInputProcessorService,
        ISocketService socketService,
        ISocketReceiverService socketReceiverService
    )
    {
        _hostApplicationLifetime = hostApplicationLifetime;

        _logger = logger;

        _consoleInputProcessService = consoleInputProcessorService;
        _socketService = socketService;
        _socketReceiverService = socketReceiverService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _socketService.ConnectAsync();

            _ = _socketReceiverService.ReceiveAsync(_socketService.Socket, cancellationToken);

            await _consoleInputProcessService.ProcessAsync(
                _socketService.Socket,
                cancellationToken
            );
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
        {
            _logger.LogInformation("Shutting down...");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _socketService.Stop();

        await base.StopAsync(cancellationToken);
    }
}
