namespace TcpServer.Services;

using System.Net.Sockets;
using Microsoft.Extensions.Logging;

public interface IMessageHandlerService
{
    public Task Handle(Socket socket, string? message);
}

public class MessageHandlerService : IMessageHandlerService
{
    private readonly ILogger<MessageHandlerService> _logger;
    private readonly IGamesService _gamesService;

    public MessageHandlerService(ILogger<MessageHandlerService> logger, IGamesService gamesService)
    {
        _logger = logger;
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
            case "leave":
                await _gamesService.LeaveGame(socket, arguments);
                break;
            case "quit":
                await _gamesService.LeaveGame(socket);
                _logger.LogInformation("Socket disconnected");
                break;
            default:
                Console.WriteLine(message);
                break;
        }
    }
}
