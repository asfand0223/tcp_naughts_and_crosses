namespace TcpClient.Services;

using System.Net.Sockets;
using TcpClient.Models;
using TcpClient.Validators;

public interface IGameService
{
    public Task CreatedGame(Socket socket, string[]? arguments);
}

public class GameService : IGameService
{
    public Game? Game { get; set; }

    private readonly ISocketWriterService _socketWriterService;
    private readonly IGameServiceValidator _gameServiceValidator;

    public GameService(
        ISocketWriterService socketWriterService,
        IGameServiceValidator gameServiceValidator
    )
    {
        _socketWriterService = socketWriterService;
        _gameServiceValidator = gameServiceValidator;
    }

    public async Task CreatedGame(Socket socket, string[]? arguments)
    {
        var gameId = await _gameServiceValidator.ValidateCreatedGame(socket, arguments);

        if (gameId is null)
        {
            return;
        }

        Game = new Game { Id = gameId.Value };
    }
}
