namespace TcpServer.Validators;

using System.Net.Sockets;
using TcpServer.Models;
using TcpServer.Services;

public interface IGamesServiceValidator
{
    public Task<Game?> ValidateJoinGame(List<Game> games, Socket socket, string[]? arguments);
    public Task<Guid?> ValidateLeaveGame(Socket socket, string[]? arguments);
}

public class GamesServiceValidator : IGamesServiceValidator
{
    private readonly ISocketWriterService _socketWriterService;

    public GamesServiceValidator(ISocketWriterService socketWriterService)
    {
        _socketWriterService = socketWriterService;
    }

    public async Task<Game?> ValidateJoinGame(List<Game> games, Socket socket, string[]? arguments)
    {
        var gameId = await ValidateJoinGameArguments(socket, arguments);
        if (gameId is null)
        {
            return null;
        }

        var game = await ValidateGame(games, socket, gameId.Value);
        if (game is null)
        {
            return null;
        }

        var hasValidGameSockets = await ValidateGameSockets(socket, game);
        if (!hasValidGameSockets)
        {
            return null;
        }

        return game;
    }

    private async Task<Guid?> ValidateJoinGameArguments(Socket socket, string[]? arguments)
    {
        if (arguments is null || arguments.Length != 1)
        {
            await _socketWriterService.WriteAsync(socket, "Usage: join [game_id]");
            return null;
        }

        if (!Guid.TryParse(arguments[0], out var gameId))
        {
            await _socketWriterService.WriteAsync(socket, $"Invalid game {gameId}");
            return null;
        }

        return gameId;
    }

    private async Task<Game?> ValidateGame(List<Game> games, Socket socket, Guid gameId)
    {
        var game = games.FirstOrDefault(g => g.Id == gameId);
        if (game is null)
        {
            await _socketWriterService.WriteAsync(socket, $"Unable to find game {gameId}");
            return null;
        }

        return game;
    }

    private async Task<bool> ValidateGameSockets(Socket socket, Game game)
    {
        if (game.PlayerOneSocket is null)
        {
            await _socketWriterService.WriteAsync(socket, "Player one has left  game {gameId}");
            return false;
        }

        if (
            socket == game.PlayerOneSocket
            || (game.PlayerTwoSocket is not null && game.PlayerTwoSocket == socket)
        )
        {
            await _socketWriterService.WriteAsync(
                socket,
                $"You have already joined game {game.Id}"
            );
            return false;
        }

        if (game.PlayerTwoSocket is not null)
        {
            await _socketWriterService.WriteAsync(
                socket,
                $"There are already two players in game {game.Id}"
            );
            return false;
        }

        return true;
    }

    public async Task<Guid?> ValidateLeaveGame(Socket socket, string[]? arguments)
    {
        var gameId = await ValidateLeaveGameArguments(socket, arguments);
        return gameId;
    }

    private async Task<Guid?> ValidateLeaveGameArguments(Socket socket, string[]? arguments)
    {
        if (arguments is null || arguments.Length != 1)
        {
            await _socketWriterService.WriteAsync(socket, "Usage: leave [game_id]");
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
