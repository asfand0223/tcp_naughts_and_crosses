namespace TcpClient.Services;

using System.Net.Sockets;
using System.Text;

public interface ISocketReceiverService
{
    public Task ReceiveAsync(Socket socket, CancellationToken cancellationToken);
}

public class SocketReceiverService : ISocketReceiverService
{
    private const int HEADER_SIZE = 4;

    private readonly IMessageHandlerService _messageHandlerService;

    public SocketReceiverService(IMessageHandlerService messageHandlerService)
    {
        _messageHandlerService = messageHandlerService;
    }

    public async Task ReceiveAsync(Socket socket, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var headerBuffer = await ReadExactAsync(socket, HEADER_SIZE, cancellationToken);
            if (headerBuffer is null)
            {
                return;
            }

            int messageLength = BitConverter.ToInt32(headerBuffer, 0);

            var messageBuffer = await ReadExactAsync(socket, messageLength, cancellationToken);
            if (messageBuffer is null)
            {
                return;
            }

            string message = Encoding.UTF8.GetString(messageBuffer, 0, messageBuffer.Length);

            await _messageHandlerService.Handle(socket, message);
        }
    }

    private async Task<byte[]?> ReadExactAsync(
        Socket socket,
        int bytesToRead,
        CancellationToken cancellationToken
    )
    {
        int bytesRead = 0;
        var buffer = new byte[bytesToRead];

        while (bytesRead < bytesToRead)
        {
            int bytesReceived = await socket.ReceiveAsync(
                buffer.AsMemory(bytesRead, bytesToRead - bytesRead),
                SocketFlags.None,
                cancellationToken
            );
            if (bytesReceived == 0)
            {
                return null;
            }

            bytesRead += bytesReceived;
        }

        return buffer;
    }
}
