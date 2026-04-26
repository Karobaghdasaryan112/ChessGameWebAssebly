using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Infrastructure.Persistance.Data;

namespace ChessGame.Infrastructure.Persistance.Repositories
{
    public class UnitOfWork(ChessGameDbContext chessGameDbContext) : IChessGameUnitOfWork
    {
        /// <summary>
        /// Asynchronously saves all changes made in the context to the underlying database.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the save operation.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result is <see langword="true"/> if at
        /// least one change was saved to the database; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await chessGameDbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
