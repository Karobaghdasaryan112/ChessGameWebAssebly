namespace ChessGame.Core.Services.Contracts.Repositories
{
    public interface IChessGameHistoryRepository
    {
        Task SaveGameHistoryAsync(int gameId, string playerWhite, string playerBlack, string result, DateTime playedOn);
        Task<List<string>> GetGameHistoryByGameIdAsync(Guid gameId);
        Task DeleteGameHistoryAsync(int gameId);
    }
}
