using ChessGame.Domain.Domain.Contracts;

namespace ChessGame.Domain.Domain.Entities
{
    public class ChessGameHistory : IEntity<int>
    {
        public int Id { get; set; }
        public Guid GameId { get; set; }
        public string FEN { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
