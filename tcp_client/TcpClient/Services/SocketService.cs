namespace TcpClient.Services;

using System.Net;
using System.Net.Sockets;

public interface ISocketService
{
    public Socket Socket { get; set; }

    public Task ConnectAsync();
    public void Stop();
}

public class SocketService : ISocketService
{
    public Socket Socket { get; set; }

    private const int HEADER_SIZE = 4;
    private IPEndPoint _endpoint;

    public SocketService()
    {
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _endpoint = new IPEndPoint(IPAddress.Loopback, 5000);
    }

    public async Task ConnectAsync()
    {
        await Socket.ConnectAsync(_endpoint);
    }

    public void Stop()
    {
        try
        {
            Socket.Shutdown(SocketShutdown.Both);
        }
        finally
        {
            Socket.Close();
        }
    }
}
