namespace TcpClient.Services;

using System.Net.Sockets;

public interface IMessageHandlerService
{
    public void Handle(Socket socket, string? message);
}

public class MessageHandlerService : IMessageHandlerService
{
    public void Handle(Socket socket, string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        Console.WriteLine(message);
    }
}
