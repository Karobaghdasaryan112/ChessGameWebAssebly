using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Services.HubServices;
using Microsoft.AspNetCore.SignalR;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
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


        //InvitationService 

        public async Task SendInviteAsync(ConnectionRequestDTO<SendInvitationRequestDTO> connectionRequestDTO)
           => await _invitationService.SendInviteAsync(connectionRequestDTO);

        public async Task<ConnectionResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>> AcceptInviteAsync(ConnectionRequestDTO<AcceptInvitationRequestDTO> acceptInvitationRequestDTO)
            => await _invitationService.AcceptInviteAsync(new ConnectionRequestDTO<AcceptInvitationRequestDTO>() { Data = new AcceptInvitationRequestDTO() { inviterUserGuid = acceptInvitationRequestDTO.Data.inviterUserGuid, receiverUserGuid = acceptInvitationRequestDTO.Data.receiverUserGuid } });

        public async Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
            => await _invitationService.CancelInviteAsync(inviterUserGuid, receiverUserGuid);

        //InvitationService 


        //GameService
        //TO:DO

        public async Task ClearGameAsync(Guid gameId)
            => await _gameService.ClearGameAsync(gameId);

        //TO:DO
        public async Task<ConnectionResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(ConnectionRequestDTO<SendGameStateReqeustDTO> gameStateReqeustDTO)
            => await _gameService.SendGameStateAsync(gameStateReqeustDTO);

        public async Task<ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(ConnectionRequestDTO<GetONlinePlayersRequestDTO> connectionRequestDTO)
            => await _gameService.GetOnlinePlayersAsync(connectionRequestDTO);

        public Task<ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(ConnectionRequestDTO<MoveRequestDTO> sendMoveConnectionRequestDTO)
            => _gameService.SendMoveAsync(sendMoveConnectionRequestDTO);

        public async Task<bool> SendIsSameFigureClickedAsync(Position selectedPosition, Position currentPosition, Guid gameId)
            => await _gameService.SendIsSameFigureClickedAsync(selectedPosition, currentPosition, gameId);

        public async Task<ConnectionResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(ConnectionRequestDTO<ClickRequestDTO> sendClickConnectionRequestDTO)
            => await _gameService.SendClickAsync(sendClickConnectionRequestDTO);

        //GameService


        //connectionService

        public async Task<ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsync(Guid currentUserGuid)
            => await _connectionService.RemoveConnectionAsUserGuidAsync(new ConnectionRequestDTO<RemoveUserConnectionRequestDTO>() { Data = new RemoveUserConnectionRequestDTO() { UserGuid = currentUserGuid } });

        public async Task<ConnectionResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(ConnectionRequestDTO<AddUserConnectionRequestDTO> addUserConnectionRequestDTO)
            => await _connectionService.AddConnectionAsync(addUserConnectionRequestDTO);

        public async Task<ConnectionResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>> GetUserConnectionAsync(Guid userGuid)
             => await _connectionService.GetUserConnection(new ConnectionRequestDTO<GetUserConnectionRequestDTO>() { Data = new GetUserConnectionRequestDTO() { UserGuid = userGuid } });
        public async Task<ConnectionResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>> SendBoardStateToClient(ConnectionRequestDTO<BoardStateRequestDTO> boardStateConnectionRequestDTO, string player, bool isMyConnection)
            => await _connectionService.SendBoardStateToClient(boardStateConnectionRequestDTO, player, isMyConnection);

        //connectionService
    }
}
