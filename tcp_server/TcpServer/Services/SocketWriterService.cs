namespace TcpServer.Services;

using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public interface ISocketWriterService
{
    public Task WriteAsync(Socket socket, string message);
}

public class SocketWriterService : ISocketWriterService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<SocketWriterService> _logger;

    public SocketWriterService(
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<SocketWriterService> logger
    )
    {
        _hostApplicationLifetime = hostApplicationLifetime;

        _logger = logger;
    }

    public async Task WriteAsync(Socket socket, string message)
    {
        try
        {
            var messageWithPrependedLengthBytes = GetMessageWithPrependedLengthBytes(message);
            await socket.SendAsync(messageWithPrependedLengthBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StartAsync");
        }
    }

    private byte[] GetMessageWithPrependedLengthBytes(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var messageLengthBytes = BitConverter.GetBytes(message.Length);
        var buffer = new byte[4 + messageBytes.Length];

        Array.Copy(messageLengthBytes, 0, buffer, 0, 4);
        Array.Copy(messageBytes, 0, buffer, 4, messageBytes.Length);

        return buffer;
    }
}
