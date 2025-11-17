using BlazorServerSideClient.Contracts.Handlers;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;

namespace BlazorServerSideClient.Services.Handlers
{
    public class InvitationHandlerService : IInvitationHandlerService
    {
        JSRunetimeService _jSRunetimeService { get; set; }
        public  Action<SendInvitationsResponseDTO> InvitationAction { get; set; }

        public InvitationHandlerService(JSRunetimeService JSRunetimeService)
        {
            _jSRunetimeService = JSRunetimeService;
        }

        public async void ReceiveInvite(UserConnectionDTO inviterUserConnection, UserConnectionDTO receiverUserCOnnection)
        {
            InvitationAction?.Invoke(new SendInvitationsResponseDTO() { InviterUserConnection = inviterUserConnection });
        }
    }
}

