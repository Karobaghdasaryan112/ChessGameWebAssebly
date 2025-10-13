using ChessGame.Domain.Domain.Contracts;

namespace ChessGame.Domain.Domain.Entities
{
    public class ChessGameHistory : IEntity
    {
        public int GameId { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
