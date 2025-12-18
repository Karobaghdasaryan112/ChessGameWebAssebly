namespace ChessGame.Domain.Domain.Contracts
{
    public interface IEntity<TKeyType>
    {
        TKeyType Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
