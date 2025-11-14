using BlazorServerSideClient.Contracts.Handlers;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;

namespace BlazorServerSideClient.Services.Handlers
{
    public class InvitationHandlerService : IInvitationHandlerService
    {
        private JSRunetimeService _jsRunetimeService { get; set; }
        public Action<SendInvitationsResponseDTO> InvitationAction { get; set; }
        public InvitationHandlerService(JSRunetimeService jSRunetimeService)
        {
            _jsRunetimeService = jSRunetimeService;
        }
        public async void ReceiveInvite(UserConnectionDTO inviterUserConnection, UserConnectionDTO receiverUserCOnnection)
        {
            InvitationAction?.Invoke(new SendInvitationsResponseDTO() { InviterUserConnection = inviterUserConnection, });
        }

    }
}

