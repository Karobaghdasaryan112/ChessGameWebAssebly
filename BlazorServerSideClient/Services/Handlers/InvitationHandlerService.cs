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
        JSRunetimeService _jSRunetimeService { get; set; }
        public Action<ConnectionResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>> OnReceived { get; set; }
        public SendInvitationsResponseDTO? lastInvite { get; set; }
        public NavigationManager _navigationManager { get; set; }
        Action<ConnectionResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>> IInvitationHandlerService.OnReceived { get => OnReceived; set => OnReceived = value; }

        public InvitationHandlerService(JSRunetimeService JSRunetimeService, NavigationManager NavigationManager)
        {
            _navigationManager = NavigationManager;
            _jSRunetimeService = JSRunetimeService;
        }

        public async void ReceiveInvite(
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
                new ConnectionResponseDTO<SendInvitationsResponseDTO, ChessGameResponseMessage>()
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

            await _jSRunetimeService.ShowInviteModal(15, inviterUserConnection.UserName);
        }
        public void InviteAcceptedAsync(
            UserConnectionDTO inviterUserConnection,
            Guid inviterUserGuid,
            UserConnectionDTO receiverUserConnection,
            Guid receiverUserGuid,
            Guid gameGuid)
        {
            _navigationManager.NavigateTo($"/game?GameId={gameGuid}&Player1={inviterUserConnection.UserName}&Player2={receiverUserConnection.UserName}", true);
        }

    }
}

