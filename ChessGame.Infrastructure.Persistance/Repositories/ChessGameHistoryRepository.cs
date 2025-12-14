using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Domain.Domain.Entities;
using ChessGame.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;

namespace ChessGame.Infrastructure.Persistance.Repositories
{
    public class ChessGameHistoryRepository(ChessGameDbContext chessGameDbContext) : IChessGameHistoryRepository
    {
        private readonly ChessGameDbContext _chessGameDbContext = chessGameDbContext;

        public Task DeleteGameHistoryAsync(int gameId)
        {

            throw new NotImplementedException();
        }

        public async Task<List<string>> GetGameHistoryByGameIdAsync(Guid gameId)
        =>
             await _chessGameDbContext.ChessGamesHistory
                .Where(chessGameHistory =>
                    chessGameHistory.GameId == gameId)
                .Select(selectedGameHistory =>
                    selectedGameHistory.FEN).ToListAsync();


        public async Task<bool> SaveGameStateAsync(ChessGameHistory chessGameHistory)
        {
            await _chessGameDbContext.ChessGamesHistory.AddAsync(chessGameHistory);
            return await _chessGameDbContext.SaveChangesAsync() > 0;
        }

        public Task<List<string>> GetGameHistoryByPlayerAsync(string player)
        {
            throw new NotImplementedException();
        }


        public Task SaveGameHistoryAsync(int gameId, string playerWhite, string playerBlack, string result, DateTime playedOn)
        {
            throw new NotImplementedException();
        }



    }
}
