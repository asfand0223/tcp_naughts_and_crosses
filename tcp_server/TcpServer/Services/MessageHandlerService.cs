namespace TcpServer.Services;

using System.Net.Sockets;

public interface IMessageHandlerService
{
    public Task Handle(Socket socket, string? message);
}

public class MessageHandlerService : IMessageHandlerService
{
    public IGamesService _gamesService;

    public MessageHandlerService(IGamesService gamesService)
    {
        _gamesService = gamesService;
    }

    public async Task Handle(Socket socket, string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        var commandAndArguments = message.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var command = commandAndArguments[0];
        var arguments = commandAndArguments[1..];

        switch (command.ToLower())
        {
            case "create":
                await _gamesService.CreateGame(socket);
                break;
            case "join":
                await _gamesService.JoinGame(socket, arguments);
                break;
            case "list":
                await _gamesService.ListAllGames(socket);
                break;
            default:
                break;
        }
    }
}
