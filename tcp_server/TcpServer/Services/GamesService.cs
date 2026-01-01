namespace TcpServer.Services;

using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using TcpServer.Models;
using TcpServer.Validators;

public interface IGamesService
{
    public List<Game> Games { get; set; }

    public Task CreateGame(Socket socket);
    public Task ListAllGames(Socket socket);
    public Task JoinGame(Socket socket, string[]? arguments);
    public Task LeaveGame(Socket socket, string[]? arguments);
    public Task LeaveGame(Socket socket);
}

public class GamesService : IGamesService
{
    public List<Game> Games { get; set; }

    private readonly ILogger<GamesService> _logger;
    private readonly ISocketWriterService _socketWriterService;
    private readonly IGamesServiceValidator _gamesServiceValidator;

    public GamesService(
        ILogger<GamesService> logger,
        ISocketWriterService socketWriterService,
        IGamesServiceValidator gamesServiceValidator
    )
    {
        Games = new List<Game>();

        _logger = logger;
        _socketWriterService = socketWriterService;
        _gamesServiceValidator = gamesServiceValidator;
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

    public async Task CreateGame(Socket socket)
    {
        var newGame = new Game
        {
            Id = Guid.NewGuid(),
            PlayerOneSocket = socket,
            GameState = GameState.CREATED,
        };
        Games.Add(newGame);

        await _socketWriterService.WriteAsync(socket, $"created {newGame.Id}");
    }

    public async Task JoinGame(Socket socket, string[]? arguments)
    {
        var game = await _gamesServiceValidator.ValidateJoinGame(Games, socket, arguments);
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

    public async Task LeaveGame(Socket socket, string[]? arguments)
    {
        var gameId = await _gamesServiceValidator.ValidateLeaveGame(socket, arguments);
        if (gameId is null)
        {
            return;
        }

        var game = Games.FirstOrDefault(g => g.Id == gameId);

        if (game is null)
        {
            await _socketWriterService.WriteAsync(socket, $"Unable to find game {gameId}");
            return;
        }

        if (game.PlayerOneSocket != socket && game.PlayerTwoSocket != socket)
        {
            await _socketWriterService.WriteAsync(socket, $"You are not in game {gameId}");
            return;
        }

        if (game.PlayerOneSocket == socket)
        {
            game.PlayerOneSocket = null;
        }
        else if (game.PlayerTwoSocket == socket)
        {
            game.PlayerTwoSocket = null;
        }

        if (game.PlayerOneSocket is null && game.PlayerTwoSocket is null)
        {
            Games.RemoveAll(g => g.Id == gameId);
        }

        await _socketWriterService.WriteAsync(socket, $"Left game {gameId}");
    }

    public async Task LeaveGame(Socket socket)
    {
        var games = Games.FindAll(g => g.PlayerOneSocket == socket || g.PlayerTwoSocket == socket);
        games.ForEach(g =>
        {
            if (g.PlayerOneSocket == socket)
            {
                g.PlayerOneSocket = null;
            }
            else if (g.PlayerTwoSocket == socket)
            {
                g.PlayerTwoSocket = null;
            }

            if (g.PlayerOneSocket == null && g.PlayerTwoSocket == null)
            {
                Games.Remove(g);
            }
        });

        await _socketWriterService.WriteAsync(socket, $"You have quit");
    }
}
