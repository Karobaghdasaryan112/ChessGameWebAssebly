using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Requests
{
    public interface IConnectionReqeustService
    {
        Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> GetUserConnection(Guid userGuid);
        Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(Guid userGuid, UserConnectionResponseDTO userConnection);
        Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsync(Guid userGuid);
    }
}
