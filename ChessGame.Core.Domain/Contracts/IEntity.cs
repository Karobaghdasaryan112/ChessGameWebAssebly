namespace ChessGame.Domain.Domain.Contracts
{
    public interface IEntity<TKeyType>
    {
        TKeyType Id { get; set; }        
    }
}
