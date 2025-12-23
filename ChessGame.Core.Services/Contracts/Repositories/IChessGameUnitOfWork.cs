namespace ChessGame.Core.Services.Contracts.Repositories
{
    public interface IChessGameUnitOfWork
    {
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
