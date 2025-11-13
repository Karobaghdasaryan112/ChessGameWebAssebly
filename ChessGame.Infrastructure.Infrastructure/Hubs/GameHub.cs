using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Services.HubServices;
using Microsoft.AspNetCore.SignalR;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Infrastructure.Infrastructure.Hubs
{
    public class GameHub : Hub
    {
        private readonly IInvitationService<GameHub> _invitationService;
        private readonly IGameService<GameHub> _gameService;
        private readonly IConnectionService<GameHub> _connectionService;
        private readonly BaseHubService<GameHub> _baseHubService;
        public GameHub(
            BaseHubService<GameHub> baseHubService,
            IInvitationService<GameHub> invitationService,
            IGameService<GameHub> gameService,
            IConnectionService<GameHub> connectionService)
        {
            _baseHubService = baseHubService;
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
            await _connectionService.RemoveConnectionAsConnectionIdAsync(
                     new RemoveUserConnectionRequestDTO()
                     {
                         ConnectionId = Context.ConnectionId
                     });

            await base.OnDisconnectedAsync(exception);
        }


        //InivtationService 
        public async Task SendInvite(UserConnectionDTO inviterUserConnection, UserConnectionDTO receiverUserConnection)
           => await _invitationService.SendInvite(
               new ConnectionRequestDTO<SendInvitationRequestDTO>()
               {
                   Data = new SendInvitationRequestDTO()
                   {
                       InviterUserConnection = inviterUserConnection,
                       ReceiverUserConnection = receiverUserConnection
                   }
               });

        public async Task<ConnectionResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>> AcceptInvite(Guid inviterUserGuid, Guid receiverUserGuid)
             => await _invitationService.AcceptInviteAsync(new ConnectionRequestDTO<AcceptInvitationRequestDTO>()
             {
                 Data = new AcceptInvitationRequestDTO()
                 {
                     inviterUserGuid = inviterUserGuid,
                     receiverUserGuid = receiverUserGuid
                 }
             });

        public async Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
            => await _invitationService.CancelInviteAsync(inviterUserGuid, receiverUserGuid);


        //GameService
        //TO:DO
        public async Task ClearGameAsync(Guid gameId)
            => await _gameService.ClearGameAsync(gameId);

        //TO:DO
        public async Task SendGameStateAsync(Guid gameId)
            => await _gameService.SendGameStateAsync(gameId);

        public async Task<IResponseTypes<Dictionary<Guid, UserConnectionDTO>, ChessGameResponseMessage>> GetOnlinePlayersAsync(Guid currentUserGuid)
            => await _gameService.GetOnlinePlayersAsync(currentUserGuid);


        //connectionService
        public async Task<ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsync(Guid currentUserGuid)
            => await _connectionService.RemoveConnectionAsUserGuidAsync(
                 new ConnectionRequestDTO<RemoveUserConnectionRequestDTO>()
                 {
                     Data = new RemoveUserConnectionRequestDTO()
                     {
                         UserGuid = currentUserGuid
                     }
                 });

        public async Task<ConnectionResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(Guid currentUserGuid, UserConnectionDTO currentUserConnection)
            => await _connectionService.AddConnectionAsync(
                new ConnectionRequestDTO<AddUserConnectionRequestDTO>()
                {
                    Data = new AddUserConnectionRequestDTO()
                    {
                        userConnection = currentUserConnection,
                        userGuid = currentUserGuid
                    }
                });

        public ConnectionResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage> GetUserConnectionAsync(Guid userGuid)
             => _connectionService.GetUserConnection(new ConnectionRequestDTO<GetUserConnectionRequestDTO>()
             {
                 Data = new GetUserConnectionRequestDTO()
                 {
                     UserGuid = userGuid
                 }
             });
    }
}
