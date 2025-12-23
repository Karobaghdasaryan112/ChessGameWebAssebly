using ChessGame.Domain.Domain.Entities;

namespace ChessGame.Core.Services.Contracts.Repositories
{
    public interface IChessGameHistoryRepository
    {
        Task<List<string>> GetGameHistoryByGameIdAsync(Guid gameId);
        Task SaveGameStateAsync(ChessGameHistory chessGameHistory);
    }
}
