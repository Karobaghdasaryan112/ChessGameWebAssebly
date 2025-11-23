using ChessGame.Domain.Domain.Contracts;

namespace ChessGame.Domain.Domain.Entities
{
    public class Game : IEntity
    {
        public Guid GameId { get; set; } = Guid.NewGuid();
        public Guid Player1 { get; set; }
        public Guid Player2 { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
