using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Domain.Domain.Entities;
using ChessGame.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;

namespace ChessGame.Infrastructure.Persistance.Repositories
{
    public class ChessGameHistoryRepository(ChessGameDbContext chessGameDbContext) : IChessGameHistoryRepository
    {
        private readonly ChessGameDbContext _chessGameDbContext = chessGameDbContext;

        public Task<List<string>> GetGameHistoryByGameIdAsync(Guid gameId)
        =>
             _chessGameDbContext.ChessGamesHistory.AsNoTracking()
                .Where(chessGameHistory => chessGameHistory.GameId == gameId)
                .Select(selectedGameHistory => selectedGameHistory.FEN).ToListAsync();

        public async Task SaveGameStateAsync(ChessGameHistory chessGameHistory)
        => await _chessGameDbContext.ChessGamesHistory.AddAsync(chessGameHistory);

    }
}
