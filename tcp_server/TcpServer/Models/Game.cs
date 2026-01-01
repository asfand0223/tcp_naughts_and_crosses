namespace TcpServer.Models;

using System.Net.Sockets;

public class Game
{
    public Guid Id { get; set; }
    public Socket? PlayerOneSocket { get; set; }
    public Socket? PlayerTwoSocket { get; set; }
    public GameState GameState { get; set; }
}
