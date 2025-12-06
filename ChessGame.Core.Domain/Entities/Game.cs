using ChessGame.Domain.Domain.Contracts;
using SharedResources.ChessGameResource.Enums.Events;

namespace ChessGame.Domain.Domain.Entities
{
    public class Game : IEntity<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        //By Default,it is white Player
        public Guid Player1 { get; set; }
        public Guid Player2 { get; set; }

        public string Player1Name { get; set; }
        public string Player2Name { get; set; }

        public Guid WinnerPlayer { get; set; } = Guid.Empty;

        public int Player1Time { get; set; }
        public int Player2Time { get; set; }

        public GameEvent GameEvent { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}