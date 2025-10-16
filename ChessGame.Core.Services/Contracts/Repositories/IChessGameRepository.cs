using SharedResources.ChessGameResource.Models;

namespace ChessGame.Core.Services.Contracts.Repositories
{
    public interface IChessGameRepository
    {
        Task<bool> CreateGame(string player1, string player2);
        Task<int> GetGameIdByPlayers(string player1, string player2);
        Task<bool> MovePiece(int gameId, string player, Block block);
        Task<bool> SubmitMove(int gameId, Position currentPosition, Position movePosition, string player);
        Task<bool> ResignGame(int gameId, string player);
        Task<bool> OfferDraw(int gameId, string player);
        Task<bool> AcceptDraw(int gameId, string player);
        Task<bool> DeclineDraw(int gameId, string player);
        Task<string> GetGameState(int gameId);
        Task<string> GetCurrentTurn(int gameId);
        Task<string> GetWinner(int gameId);
        Task<bool> IsPlayerInGame(int gameId, string player);
        Task<bool> IsGameOver(int gameId);
    }
}
