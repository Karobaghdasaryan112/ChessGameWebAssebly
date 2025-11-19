using SharedResources.ChessGameResource.Models;

namespace ChessGame.Core.Services.Contracts.BoardServices
{
    public interface IBoardService
    {
        Task<Guid> InitializeBoardAsync(Guid player1Id, Guid player2Id);
        Task<bool> SubmitMoveAsync(int gameId, Position currentPosition, Position movePosition, string player);
    }
}
