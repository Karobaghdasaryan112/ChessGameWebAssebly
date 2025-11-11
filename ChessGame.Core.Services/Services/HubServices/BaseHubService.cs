using Microsoft.AspNetCore.SignalR;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using System.Collections.Concurrent;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class BaseHubService<THub> where THub : Microsoft.AspNetCore.SignalR.Hub
    {
        public readonly IHubContext<THub> _hubContext;
        public BaseHubService(IHubContext<THub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendUsersChange(
            ConcurrentDictionary<Guid, UserConnectionResponseDTO> connections) =>
            await _hubContext.Clients.All.SendAsync("ReceiveUpdatedUsers", connections);

        public async Task SendAcceptedInviteAsync(
            string conectionId,
            Guid gameId) =>
            await _hubContext.Clients.Client(conectionId).SendAsync("InviteAccepted", gameId);

        public async Task SendInviteAsync(
            string connectionId,
            UserConnectionResponseDTO inviterUserConnection,
            UserConnectionResponseDTO receiverUserConnection) =>
            await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveInvite", inviterUserConnection, receiverUserConnection);


        public async Task AddToGroupAsync(string gruopName,string connectionId) 
            => await _hubContext.Groups.AddToGroupAsync(connectionId, gruopName);
    }
}
