using ChessGame.Domain.Domain.Entities;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;

namespace ChessGame.Core.Services.Contracts.Repositories
{
    public interface IChessGameRepository
    {
        Task<bool> CreateGame(
            Guid player1,
            Guid player2,
            GameEvent gameEvent,
            string player1Name, 
            string player2Name,
            int player1Time, int player2Time);
        Task<Guid> GetGameIdByPlayers(Guid player1, Guid player2);

        Task<bool> SaveGameResult(Guid winnerPlayer, Guid gameId);

        Task<List<Game>> GetGameStatesByCurrentAndOpponentIdsPagination(Guid currentPlayerGuid, Guid opponentPlayerGuid,
            int currentPage, int pageSize);
        Task<List<OpponentsHistoryDTO>> GetAllGames(Guid currentPlayerId);
    }
}
