using ChessGame.Core.Services.Contracts.Hub;
using Microsoft.AspNetCore.SignalR;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Infrastructure.Infrastructure.Hubs
{
    public class GameHub : Hub
    {
        private readonly IInvitationService<GameHub> _invitationService;
        private readonly IGameService<GameHub> _gameService;
        private readonly IConnectionService<GameHub> _connectionService;
        public GameHub(
            IInvitationService<GameHub> invitationService,
            IGameService<GameHub> gameService,
            IConnectionService<GameHub> connectionService)
        {
            _connectionService = connectionService;
            _invitationService = invitationService;
            _gameService = gameService;
        }
        public override async Task OnConnectedAsync()
        {

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _connectionService.RemoveConnectionAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }


        //InivtationService 
        public async Task SendInvite(UserConnectionResponseDTO inviterUserConnection, UserConnectionResponseDTO receiverUserConnection)
           => await _invitationService.SendInvite(inviterUserConnection, receiverUserConnection);

        public async Task<IResponseTypes<InvitationResponseDTO, ChessGameResponseMessage>> AcceptInvite(Guid inviterUserGuid, Guid receiverUserGuid)
             => await _invitationService.AcceptInviteAsync(inviterUserGuid, receiverUserGuid);

        public async Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
            => await _invitationService.CancelInviteAsync(inviterUserGuid, receiverUserGuid);


        //GameService
        //TO:DO
        public async Task ClearGameAsync(Guid gameId)
            => await _gameService.ClearGameAsync(gameId);

        //TO:DO
        public async Task SendGameStateAsync(Guid gameId)
            => await _gameService.SendGameStateAsync(gameId);

        public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(Guid currentUserGuid)
            => await _gameService.GetOnlinePlayersAsync(currentUserGuid);


        //connectionService
        public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsync(Guid currentUserGuid)
            => await _connectionService.RemoveConnectionAsync(currentUserGuid);

        public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(Guid currentUserGuid, UserConnectionResponseDTO currentUserConnection)
            => await _connectionService.AddConnectionAsync(currentUserGuid, currentUserConnection);

        public IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage> GetUserConnectionAsync(Guid userGuid)
             => _connectionService.GetUserConnection(userGuid);

    }
}
