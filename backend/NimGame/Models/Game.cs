namespace NimGame.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int[] Columns { get; set; }
        public string CurrentPlayer { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }

        public Game()
        {
            // Para Entity Framework
        }

        public Game(string player1, string player2)
        {
            Player1 = player1;
            Player2 = player2;
            CurrentPlayer = player1;
            Columns = new[] { 3, 4, 5, 4, 3 };
        }
    }
}
