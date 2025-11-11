using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class InvitationRequestService : IInivitationReqeustService
    {
        private readonly SignalRService _signalRService;
        public InvitationRequestService(SignalRService signalRService)
        {
            _signalRService = signalRService;
        }

        public async Task SendInviteAsync(Guid inviterPlayerId, Guid receiverPlayerId)
        {
            var hubConnection = await _signalRService.GetHubConnection();
            await hubConnection.InvokeAsync("SendInvite", inviterPlayerId, receiverPlayerId);
        }
        public async Task CancelInviteAsync(Guid inviterPlayerGuid, Guid receiverUserGuid)
        {
            var hubConnection = await _signalRService.GetHubConnection();
            //TO:do : Implement CancelInviteAsync in Hub
            await hubConnection.InvokeAsync("CancelInviteAsync", inviterPlayerGuid, receiverUserGuid);
        }

        public async Task<IResponseTypes<InvitationResponseDTO, ChessGameResponseMessage>> AcceptInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
        {
            var hubConnection = await _signalRService.GetHubConnection();
            return await hubConnection.
                InvokeAsync<
                    IResponseTypes<
                        InvitationResponseDTO, 
                        ChessGameResponseMessage>>
                        ("AcceptInvite", inviterUserGuid, receiverUserGuid);
        }

    }
}
