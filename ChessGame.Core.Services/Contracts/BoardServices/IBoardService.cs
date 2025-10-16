using SharedResources.ChessGameResource.Models;

namespace ChessGame.Core.Services.Contracts.BoardServices
{
    public interface IBoardService
    {
        Task<int> InitializeBoardAsync(string player1Id, string player2Id);
        Task<bool> SubmitMoveAsync(int gameId, Position currentPosition, Position movePosition, string player);
    }
}
