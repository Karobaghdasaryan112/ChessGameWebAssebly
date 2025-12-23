using ChessGame.Infrastructure.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class BaseHubService(IHubContext<GameHub> hubContext)
    {
        public readonly IHubContext<GameHub> _hubContext = hubContext;

        public async Task SendUsersChange(
            KeyValuePair<Guid, UserConnectionDTO> connections) =>
            await _hubContext.Clients.All.SendAsync("ReceiveUpdatedUsers", connections);

        public async Task SendAcceptedInviteAsync(string conectionId, UserConnectionDTO inviterUserConnection, Guid inviterUserGuid, UserConnectionDTO receiverUserConnection, Guid receiverUserGuid, Guid gameId)
            =>
            await _hubContext.Clients.Group(gameId.ToString()).SendAsync("InviteAcceptedAsync", inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid, gameId);

        public async Task SendPalyersInformationAsync(ResponseDTO<ReceivePlayersResponseDTO, ChessGameResponseMessage> receivePlayersResponseDTO) =>
            await _hubContext.Clients.Group(receivePlayersResponseDTO.Data.Player1_UserConnectionDTO.GameId.ToString()).SendAsync("ReseivePlayersAsync", receivePlayersResponseDTO);

        public async Task SendInviteAsync(SendInvitationRequestDTO connectionRequestDTO) =>
            await _hubContext.Clients.Client(connectionRequestDTO.ReceiverUserConnection.ConnectionId).SendAsync("ReceiveInvite", connectionRequestDTO.InviterUserConnection, connectionRequestDTO.InviterPlayerId, connectionRequestDTO.ReceiverUserConnection, connectionRequestDTO.ReceiverPlayerId);


        public async Task AddToGroupAsync(string groupName, string connectionId)
            => await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);

        public async Task RemoveFromGroupAsync(string groupName, List<string> connectionIds) =>
            connectionIds.ForEach(async connectionId => await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName));

        public async Task ReceiveBoardUpdateAsync(ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage> connectionResponseDTO)
           => await _hubContext.Clients.Client(connectionResponseDTO.Data.OpponentConnectionId).SendAsync("ReceiveBoardUpdateAsync", connectionResponseDTO);
    }
}
