using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

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

    }
}
