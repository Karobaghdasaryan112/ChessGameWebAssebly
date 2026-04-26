using BlazorServerSideClient.Contracts.Handlers;
using Microsoft.AspNetCore.Components;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Handlers
{
    public class InvitationHandlerService : IInvitationHandlerService
    {
        JSRunetimeService _JSRunetimeService { get; set; }
        public Action<ResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>> OnReceived { get; set; }
        public SendInvitationsResponseDTO? lastInvite { get; set; }
        private NavigationManager _navigationManager { get; set; }

        Action<ResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>> IInvitationHandlerService.OnReceived
        {
            get => OnReceived;
            set => OnReceived = value;
        }

        public InvitationHandlerService(JSRunetimeService JSRunetimeService, NavigationManager NavigationManager)
        {
            _navigationManager = NavigationManager;
            _JSRunetimeService = JSRunetimeService;
        }

        public async Task ReceiveInvite(
            UserConnectionDTO inviterUserConnection,
            Guid inviterUserGuid,
            UserConnectionDTO receiverUserConnection,
            Guid receiverUserGuid)
        {
            lastInvite = new SendInvitationsResponseDTO()
            {
                InviterUserConnection = inviterUserConnection,
            };

            OnReceived?.Invoke(
                new ResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>()
                {
                    Data = new SendInvitationsResponseDTO()
                    {
                        InviterUserConnection = inviterUserConnection,
                        InviterUserGuid = inviterUserGuid,
                        ReceiverUserConnection = receiverUserConnection,
                        ReceiverUserGuid = receiverUserGuid
                    },
                    Message = ChessGameResponseMessage.SuccessInvitation,
                });

            await _JSRunetimeService.ShowInviteModal(inviterUserConnection.UserName);
        }

        public void InviteAcceptedAsync(
            UserConnectionDTO inviterUserConnection,
            Guid inviterUserGuid,
            UserConnectionDTO receiverUserConnection,
            Guid receiverUserGuid,
            Guid gameGuid)
        {
            var url = $"/game?GameId={gameGuid}" +
                      $"&Player1={inviterUserConnection.UserName}" +
                      $"&Player2={receiverUserConnection.UserName}";

             _ = _JSRunetimeService.NavigateTo(url);
        }
    }
}