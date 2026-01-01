namespace TcpServer.Services;

using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

public interface IListenSocketService
{
    public void Listen(int port);
    public Task AcceptClientsAsync(CancellationToken cancellationToken);
    public void Stop();
}

public class ListenSocketService : IListenSocketService
{
    private const int HEADER_SIZE = 4;

    private readonly ILogger<ListenSocketService> _logger;
    private readonly ISocketReceiverService _socketReceiverService;

    private Socket _listenSocket;

    public ListenSocketService(
        ILogger<ListenSocketService> logger,
        ISocketReceiverService socketReceiverService
    )
    {
        _logger = logger;
        _socketReceiverService = socketReceiverService;

        _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public void Listen(int port)
    {
        var listenEndpoint = new IPEndPoint(IPAddress.Any, port);
        _listenSocket.Bind(listenEndpoint);
        _listenSocket.Listen(10);
    }

    public async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var clientSocket = await _listenSocket.AcceptAsync(cancellationToken);

            _ = _socketReceiverService.ReceiveAsync(clientSocket, cancellationToken);
        }
    }

    public void Stop()
    {
        try
        {
            _listenSocket.Shutdown(SocketShutdown.Both);
        }
        finally
        {
            _listenSocket.Close();
        }
    }
}
