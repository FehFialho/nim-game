using NimGame.Models;

namespace NimGame.Services
{
    public class GameService
    {
        public Game CreateNewGame(string player1, string player2)
        {
            return new Game(player1, player2);
        }

        // Aqui você pode implementar regras, jogadas, etc.
    }
}
