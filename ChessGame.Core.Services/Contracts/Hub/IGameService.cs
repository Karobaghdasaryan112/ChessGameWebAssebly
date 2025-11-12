using Microsoft.AspNetCore.SignalR;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IGameService<T> where T : Microsoft.AspNetCore.SignalR.Hub
    {
        Task<IResponseTypes<Dictionary<Guid, UserConnectionResponseDTO>, ChessGameResponseMessage>> GetOnlinePlayersAsync(Guid currentUserGuid);
        Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(Guid gameId);
        Task ClearGameAsync(Guid gameId);
    }
}
