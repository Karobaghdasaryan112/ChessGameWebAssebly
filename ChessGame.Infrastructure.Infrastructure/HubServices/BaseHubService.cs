using ChessGame.Infrastructure.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using SharedResources.ChessGameResource.Enums.Users;
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
            KeyValuePair<Guid, UserConnectionDTO> connections, OnlinePlayerChangeType onlinePlayerChangeType) =>
            await _hubContext.Clients.All.SendAsync("ReceiveUpdatedUsers", connections, onlinePlayerChangeType);

        public async Task SendAcceptedInviteAsync(UserConnectionDTO inviterUserConnection,
            Guid inviterUserGuid, UserConnectionDTO receiverUserConnection, Guid receiverUserGuid, Guid gameId)
        {
            await _hubContext.Clients.Client(inviterUserConnection.ConnectionId).SendAsync("InviteAcceptedAsync",
                inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid, gameId);
            
            await _hubContext.Clients.Client(receiverUserConnection.ConnectionId).SendAsync("InviteAcceptedAsync",
                inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid, gameId);
        }


        public async Task SendPalyersInformationAsync(
            ResponseDTO<ReceivePlayersResponseDTO, ChessGameResponseMessage> receivePlayersResponseDTO) =>
            await _hubContext.Clients.Group(receivePlayersResponseDTO.Data.Player1_UserConnectionDTO.GameId.ToString())
                .SendAsync("ReseivePlayersAsync", receivePlayersResponseDTO);

        public async Task SendInviteAsync(SendInvitationRequestDTO connectionRequestDTO) =>
            await _hubContext.Clients.Client(connectionRequestDTO.ReceiverUserConnection.ConnectionId)
                .SendAsync(
                    "ReceiveInvite",
                    connectionRequestDTO.InviterUserConnection,
                    connectionRequestDTO.InviterPlayerId,
                    connectionRequestDTO.ReceiverUserConnection,
                    connectionRequestDTO.ReceiverPlayerId);


        public async Task AddToGroupAsync(string groupName, string connectionId)
            => await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);

        public async Task RemoveFromGroupAsync(string groupName, List<string> connectionIds) =>
            connectionIds.ForEach(async connectionId =>
                await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName));

        public async Task RequestTrainingGameAsync(TrainingGameResponseDTO trainingGameResponseDTO)
            => await _hubContext.Clients.Client(trainingGameResponseDTO.ClientConnectionId)
                .SendAsync("ReceiveTrainingGameRequestAsync", trainingGameResponseDTO);

        public async Task ReceiveBoardUpdateAsync(ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage> connectionResponseDTO)
            => await _hubContext.Clients.Client(connectionResponseDTO.Data.OpponentConnectionId)
                .SendAsync("ReceiveBoardUpdateAsync", connectionResponseDTO);

        public async Task NotifyOpponentUserDisconnected(KeyValuePair<Guid, UserConnectionDTO> opponentUserConnection)
        {
            await _hubContext.Clients.Client(opponentUserConnection.Value.ConnectionId)
                .SendAsync("DisconnectedNotification", opponentUserConnection);
        }
    }
}