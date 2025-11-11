using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Requests
{
    public interface IInivitationReqeustService
    {
        Task SendInviteAsync(Guid inviterPlayerId, Guid receiverPlayerId);
        Task CancelInviteAsync(Guid inviterPlayerGuid, Guid receiverUserGuid);
        Task<IResponseTypes<InvitationResponseDTO, ChessGameResponseMessage>> AcceptInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid);
    }
}
