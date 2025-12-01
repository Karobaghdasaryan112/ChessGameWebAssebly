using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Handlers
{
    public interface IInvitationHandlerService
    {
        SendInvitationsResponseDTO? lastInvite { get; set; }
        Action<ConnectionResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>> OnReceived { get; set; }

        void ReceiveInvite(UserConnectionDTO inviterUserConnection, Guid inviterUserGuid, UserConnectionDTO receiverUserConnection, Guid receiverUserGuid);
        void InviteAcceptedAsync(UserConnectionDTO inviterUserConnection, Guid inviterUserGuid, UserConnectionDTO receiverUserConnection, Guid receiverUserGuid, Guid gameGuid);
    }
}
