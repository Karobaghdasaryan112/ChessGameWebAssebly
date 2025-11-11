using Microsoft.AspNetCore.SignalR;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IInvitationService<T> where T : Microsoft.AspNetCore.SignalR.Hub
    {

        Task<IResponseTypes<InvitationResponseDTO, ChessGameResponseMessage>> AcceptInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid);
        Task SendInvite(UserConnectionResponseDTO inviterUserConnection, UserConnectionResponseDTO receiverUserConnection);
        Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid);
    }
}
