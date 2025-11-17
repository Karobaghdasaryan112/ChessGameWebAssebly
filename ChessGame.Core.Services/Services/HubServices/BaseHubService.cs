using Microsoft.AspNetCore.SignalR;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;

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
            KeyValuePair<Guid, UserConnectionDTO> connections) =>
            await _hubContext.Clients.All.SendAsync("ReceiveUpdatedUsers", connections);

        public async Task SendAcceptedInviteAsync(
            string conectionId,
            UserConnectionDTO inviterUserConnection,
            Guid inviterUserGuid,
            UserConnectionDTO receiverUserConnection,
            Guid receiverUserGuid,
            Guid gameId) =>
            await _hubContext.
            Clients.
            Group(gameId.ToString()).
            SendAsync(
                "InviteAcceptedAsync",
                inviterUserConnection,
                inviterUserGuid,
                receiverUserConnection,
                receiverUserGuid,
                gameId);

        public async Task SendInviteAsync(
            ConnectionRequestDTO<SendInvitationRequestDTO> connectionRequestDTO) =>
            await _hubContext.
            Clients.
            Client(connectionRequestDTO.Data.ReceiverUserConnection.ConnectionId).
            SendAsync(
                "ReceiveInvite",
                connectionRequestDTO.Data.InviterUserConnection,
                connectionRequestDTO.Data.InviterPlayerId,
                connectionRequestDTO.Data.ReceiverUserConnection,
                connectionRequestDTO.Data.ReceiverPlayerId);


        public async Task AddToGroupAsync(string gruopName, string connectionId)
            => await _hubContext.Groups.AddToGroupAsync(connectionId, gruopName);
    }
}
