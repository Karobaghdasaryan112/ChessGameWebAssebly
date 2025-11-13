using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IGameService<T> where T : Microsoft.AspNetCore.SignalR.Hub
    {
        Task<IResponseTypes<Dictionary<Guid, UserConnectionDTO>, ChessGameResponseMessage>> GetOnlinePlayersAsync(Guid currentUserGuid);
        Task<IResponseTypes<UserConnectionDTO, ChessGameResponseMessage>> SendGameStateAsync(Guid gameId);
        Task ClearGameAsync(Guid gameId);
    }
}
