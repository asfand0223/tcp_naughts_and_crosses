namespace TcpClient.Validators;

using System.Net.Sockets;
using TcpClient.Services;

public interface IGameServiceValidator
{
    public Task<Guid?> ValidateCreatedGame(Socket socket, string[]? arguments);
}

public class GameServiceValidator : IGameServiceValidator
{
    private readonly ISocketWriterService _socketWriterService;

    public GameServiceValidator(ISocketWriterService socketWriterService)
    {
        _socketWriterService = socketWriterService;
    }

    public async Task<Guid?> ValidateCreatedGame(Socket socket, string[]? arguments)
    {
        var gameId = await ValidateCreatedGameArguments(socket, arguments);

        return gameId;
    }

    private async Task<Guid?> ValidateCreatedGameArguments(Socket socket, string[]? arguments)
    {
        if (arguments is null || arguments.Length != 1)
        {
            await _socketWriterService.WriteAsync(socket, "Usage: created [game_id]");
            return null;
        }

        if (!Guid.TryParse(arguments[0], out var gameId))
        {
            await _socketWriterService.WriteAsync(socket, $"Invalid game {gameId}");
            return null;
        }

        return gameId;
    }
}
