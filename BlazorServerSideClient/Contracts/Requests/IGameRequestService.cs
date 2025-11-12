using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Requests
{
    public interface IGameRequestService
    {
        Task<IResponseTypes<Dictionary<Guid, UserConnectionResponseDTO>, ChessGameResponseMessage>> GetOnlinePlayersAsync(Guid currentUserGuid);
        Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(Guid gameId);
        Task ClearGameAsync(Guid gameId);
    }
}
