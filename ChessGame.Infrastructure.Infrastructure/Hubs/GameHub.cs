using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Services.HubServices;
using Microsoft.AspNetCore.SignalR;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.InvitationRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.InvitationResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Infrastructure.Infrastructure.Hubs
{
    public class GameHub(
        BaseHubService baseHubService,
        IInvitationService invitationService,
        IGameService gameService,
        IConnectionService connectionService)
        : Hub
    {
        private readonly BaseHubService _baseHubService = baseHubService;

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine(10);
            await connectionService.RemoveConnectionAsConnectionIdAsync(
                     new RemoveUserConnectionRequestDTO()
                     {
                         ConnectionId = Context.ConnectionId
                     });
            await base.OnDisconnectedAsync(exception);
        }


        //InvitationService 

        public async Task SendInviteAsync(SendInvitationRequestDTO connectionRequestDTO)
           => await invitationService.SendInviteAsync(connectionRequestDTO);

        public async Task<ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>> AcceptInviteAsync(AcceptInvitationRequestDTO acceptInvitationRequestDTO)
            => await invitationService.AcceptInviteAsync(new AcceptInvitationRequestDTO() { inviterUserGuid = acceptInvitationRequestDTO.inviterUserGuid, receiverUserGuid = acceptInvitationRequestDTO.receiverUserGuid });

        public async Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
            => await invitationService.CancelInviteAsync(inviterUserGuid, receiverUserGuid);

        //InvitationService 


        //GameService

        public async Task ClearGameAsync(Guid gameId)
            => await gameService.ClearGameAsync(gameId);

        public async Task<ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(SendGameStateReqeustDTO gameStateReqeustDTO)
            => await gameService.SendGameStateAsync(gameStateReqeustDTO);

        public async Task<ResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(GetONlinePlayersRequestDTO connectionRequestDTO)
            => await gameService.GetOnlinePlayersAsync(connectionRequestDTO);

        public Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(MoveRequestDTO sendMoveConnectionRequestDTO)
            => gameService.SendMoveAsync(sendMoveConnectionRequestDTO);

        public async Task<bool> SendIsSameFigureClickedAsync(Position selectedPosition, Position currentPosition, Guid gameId)
            => await gameService.SendIsSameFigureClickedAsync(selectedPosition, currentPosition, gameId);

        public async Task<ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(ClickRequestDTO sendClickConnectionRequestDTO)
            => await gameService.SendClickAsync(sendClickConnectionRequestDTO);

        public async Task<ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>> RequestTrainingGameAsync(
            TrainingGameRequestDTO trainingGameRequestDTO) => await gameService.RequestTrainingGameAsync(trainingGameRequestDTO);

        //GameService


        //connectionService

        public async Task<ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsync(Guid currentUserGuid)
            => await connectionService.RemoveConnectionAsUserGuidAsync( new RemoveUserConnectionRequestDTO() { UserGuid = currentUserGuid });

        public async Task<ResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(AddUserConnectionRequestDTO addUserConnectionRequestDTO)
            => await connectionService.AddConnectionAsync(addUserConnectionRequestDTO);

        public async Task<ResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>> GetUserConnectionAsync(Guid userGuid)
             => await connectionService.GetUserConnection(new GetUserConnectionRequestDTO() { UserGuid = userGuid });
        public async Task<ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>> SendBoardStateToClient(BoardStateRequestDTO boardStateConnectionRequestDTO, string player, bool isMyConnection)
            => await connectionService.SendBoardStateToClient(boardStateConnectionRequestDTO, player, isMyConnection);

        //connectionService





    }
}
