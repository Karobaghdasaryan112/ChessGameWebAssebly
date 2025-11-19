using SharedResources.ChessGameResource.Models;

namespace ChessGame.Core.Services.Contracts.Repositories
{
    public interface IChessGameRepository
    {
        Task<bool> CreateGame(Guid player1, Guid player2);
        Task<Guid> GetGameIdByPlayers(Guid player1, Guid player2);
        Task<bool> MovePiece(Guid gameId, string player, Block block);
        Task<bool> SubmitMove(Guid gameId, Position currentPosition, Position movePosition, string player);
        Task<bool> ResignGame(Guid gameId, string player);
        Task<bool> OfferDraw(Guid gameId, string player);
        Task<bool> AcceptDraw(Guid gameId, string player);
        Task<bool> DeclineDraw(Guid gameId, string player);
        Task<string> GetGameState(Guid gameId);
        Task<string> GetCurrentTurn(Guid gameId);
        Task<string> GetWinner(Guid gameId);
        Task<bool> IsPlayerInGame(Guid gameId, string player);
        Task<bool> IsGameOver(Guid gameId);
    }
}
