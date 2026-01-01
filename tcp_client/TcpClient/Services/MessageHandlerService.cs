namespace TcpClient.Services;

using System.Net.Sockets;

public interface IMessageHandlerService
{
    public Task Handle(Socket socket, string? message);
}

public class MessageHandlerService : IMessageHandlerService
{
    private readonly IGameService _gameService;

    public MessageHandlerService(IGameService gameService)
    {
        _gameService = gameService;
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
            case "created":
                await _gameService.CreatedGame(socket, arguments);
                break;
            default:
                Console.WriteLine(message);
                break;
        }
    }
}
