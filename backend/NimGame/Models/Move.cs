namespace NimGame.Models
{
    public class Move
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string? Player { get; set; }
        public int Pile { get; set; }
        public int Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
