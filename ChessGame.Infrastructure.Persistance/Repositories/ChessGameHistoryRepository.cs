using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Infrastructure.Persistance.Data;

namespace ChessGame.Infrastructure.Persistance.Repositories
{
    public class ChessGameHistoryRepository : IChessGameHistoryRepository
    {
        private readonly ChessGameDbContext _chessGameDbContext;
        public ChessGameHistoryRepository(ChessGameDbContext chessGameDbContext)
        {
            _chessGameDbContext = chessGameDbContext;
        }

        public Task DeleteGameHistoryAsync(int gameId)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetGameHistoryAsync(int gameId)
        {
            throw new NotImplementedException();
        }

        public Task SaveGameHistoryAsync(int gameId, string playerWhite, string playerBlack, string result, DateTime playedOn)
        {
            throw new NotImplementedException();
        }

    }
}
