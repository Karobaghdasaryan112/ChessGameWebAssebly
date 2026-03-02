using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Handlers
{
    public interface IInvitationHandlerService
    {
        SendInvitationsResponseDTO? lastInvite { get; set; }
        Action<ResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>> OnReceived { get; set; }

        Task ReceiveInvite(UserConnectionDTO inviterUserConnection, Guid inviterUserGuid, UserConnectionDTO receiverUserConnection, Guid receiverUserGuid);
        void InviteAcceptedAsync(UserConnectionDTO inviterUserConnection, Guid inviterUserGuid, UserConnectionDTO receiverUserConnection, Guid receiverUserGuid, Guid gameGuid);
    }
}
