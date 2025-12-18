using ChessGame.Domain.Domain.Contracts;
using SharedResources.ChessGameResource.Enums.Events;

namespace ChessGame.Domain.Domain.Entities
{
    public class Game : IEntity<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid Player1 { get; set; }
        public Guid Player2 { get; set; }

        public string Player1Name { get; set; } = string.Empty;
        public string Player2Name { get; set; } = string.Empty;

        public Guid WinnerPlayer { get; set; } = Guid.Empty;

        public int Player1Time { get; set; } = 0;
        public int Player2Time { get; set; } = 0;

        public GameEvent GameEvent { get; set; } = GameEvent.None;

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}