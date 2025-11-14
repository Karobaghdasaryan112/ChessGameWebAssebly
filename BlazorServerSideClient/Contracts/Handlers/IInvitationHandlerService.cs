using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;

namespace BlazorServerSideClient.Contracts.Handlers
{
    public interface IInvitationHandlerService
    {
        Action<SendInvitationsResponseDTO> InvitationAction { get; set; }
        void ReceiveInvite(UserConnectionDTO inviterUserConnection, UserConnectionDTO receiverUserCOnnection);

    }
}
