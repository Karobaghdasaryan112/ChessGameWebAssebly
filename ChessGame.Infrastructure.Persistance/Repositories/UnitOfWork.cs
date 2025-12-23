using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;

namespace ChessGame.Infrastructure.Persistance.Repositories
{
    public class UnitOfWork(ChessGameDbContext chessGameDbContext) : IChessGameUnitOfWork
    {
        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await chessGameDbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
