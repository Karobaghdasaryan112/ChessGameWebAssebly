using ChessGame.Domain.Domain.Contracts;

namespace ChessGame.Domain.Domain.Entities
{
    public class Game : IEntity
    {
        public Guid GameId { get; set; } = Guid.NewGuid();

        //By Default,it is white Player
        public Guid Player1 { get; set; }
        public Guid Player2 { get; set; }
        public Guid WinnerPlayer { get; set; } = Guid.Empty;

        // FEN Notation of the current game state
        public int Player1Time { get; set; }
        public int Player2Time { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}