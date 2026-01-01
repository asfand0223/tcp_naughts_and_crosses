namespace TcpServer.Services;

using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using TcpServer.Models;

public interface IGamesService
{
    public List<Game> Games { get; set; }

    public Task CreateGame(Socket socket);
    public Task ListAllGames(Socket socket);
    public Task JoinGame(Socket socket, string[]? arguments);
}

public class GamesService : IGamesService
{
    public List<Game> Games { get; set; }

    private readonly ILogger<GamesService> _logger;
    private readonly ISocketWriterService _socketWriterService;

    public GamesService(ILogger<GamesService> logger, ISocketWriterService socketWriterService)
    {
        Games = new List<Game>();
        _logger = logger;
        _socketWriterService = socketWriterService;
    }

    public async Task CreateGame(Socket socket)
    {
        var newGame = new Game
        {
            Id = Guid.NewGuid(),
            PlayerOneSocket = socket,
            GameState = GameState.CREATED,
        };
        Games.Add(newGame);

        await _socketWriterService.WriteAsync(socket, $"Created game with id {newGame.Id}");
    }

    public async Task ListAllGames(Socket socket)
    {
        if (Games.Count() == 0)
        {
            await _socketWriterService.WriteAsync(socket, "===NO GAMES FOUND===");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("Games:");
        foreach (var game in Games)
        {
            sb.AppendLine($"  - {game.Id}");
        }

        await _socketWriterService.WriteAsync(socket, sb.ToString());
    }

    public async Task JoinGame(Socket socket, string[]? arguments)
    {
        var game = await ValidateJoinGame(socket, arguments);
        if (game is null)
        {
            return;
        }

        game.PlayerTwoSocket = socket;

        await _socketWriterService.WriteAsync(
            game.PlayerOneSocket!,
            $"Player has joined game {game.Id}"
        );
        await _socketWriterService.WriteAsync(game.PlayerTwoSocket, $"Joined game {game.Id}");
    }

    public async Task<Game?> ValidateJoinGame(Socket socket, string[]? arguments)
    {
        var gameId = await ValidateJoinGameArguments(socket, arguments);
        if (gameId is null)
        {
            return null;
        }

        var game = await ValidateGame(socket, gameId.Value);
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

    public async Task<Guid?> ValidateJoinGameArguments(Socket socket, string[]? arguments)
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

    public async Task<Game?> ValidateGame(Socket socket, Guid gameId)
    {
        var game = Games.FirstOrDefault(g => g.Id == gameId);
        if (game is null)
        {
            await _socketWriterService.WriteAsync(socket, $"Unable to find game {gameId}");
            return null;
        }

        return game;
    }

    public async Task<bool> ValidateGameSockets(Socket socket, Game game)
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
}
