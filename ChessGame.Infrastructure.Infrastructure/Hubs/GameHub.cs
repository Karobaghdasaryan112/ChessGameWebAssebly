using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.PipeLine.PipeLineHelper;
using ChessGame.Core.Services.Services.HubServices;
using Microsoft.AspNetCore.SignalR;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
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
using System.Net;

namespace ChessGame.Infrastructure.Infrastructure.Hubs
{
    public class GameHub(
        BaseHubService baseHubService,
        IInvitationService invitationService,
        IGameService gameService,
        IConnectionService connectionService,
        PipeLineExecutionHelper pipeLineHelper)
        : Hub
    {
        //Private PipeLineBuilder;
        //Initialize in OnConnectionAsync Method to Set up Builder


        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        //InvitationService 

        public async Task SendInviteAsync(SendInvitationRequestDTO connectionRequestDTO)

          => await pipeLineHelper.Execute(
              connectionRequestDTO,
              Context,
            () => invitationService.SendInviteAsync(connectionRequestDTO));



        public async Task<ResponseDTO<AcceptInvitationResponseDTO, ChessGameResponseMessage>> AcceptInviteAsync(
            AcceptInvitationRequestDTO acceptInvitationRequestDTO)
            => await invitationService.AcceptInviteAsync(new AcceptInvitationRequestDTO()
            {
                inviterUserGuid = acceptInvitationRequestDTO.inviterUserGuid,
                receiverUserGuid = acceptInvitationRequestDTO.receiverUserGuid
            });

        public async Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
            => await invitationService.CancelInviteAsync(inviterUserGuid, receiverUserGuid);

        //InvitationService 


        //GameService

        public async Task ClearGameAsync(Guid gameId)
            => await gameService.ClearGameAsync(gameId);

        public async Task<ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(
            SendGameStateReqeustDTO gameStateReqeustDTO)
            => await gameService.SendGameStateAsync(gameStateReqeustDTO);

        public async Task<ResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(
            GetONlinePlayersRequestDTO connectionRequestDTO)
            => await gameService.GetOnlinePlayersAsync(connectionRequestDTO);

        public Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(
            MoveRequestDTO sendMoveConnectionRequestDTO)
            => gameService.SendMoveAsync(sendMoveConnectionRequestDTO);

        public async Task<bool> SendIsSameFigureClickedAsync(SameFigureRequest sameFigureRequest)
            => await gameService.SendIsSameFigureClickedAsync(sameFigureRequest);

        public async Task<ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(
            ClickRequestDTO sendClickConnectionRequestDTO)
            => await gameService.SendClickAsync(sendClickConnectionRequestDTO);

        public async Task<ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>> RequestTrainingGameAsync(
            TrainingGameRequestDTO trainingGameRequestDTO) =>
            await gameService.RequestTrainingGameAsync(trainingGameRequestDTO);

        //GameService


        //connectionService

        public async Task<ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsync(
            Guid currentUserGuid)
            => await connectionService.RemoveConnectionAsUserGuidAsync(new RemoveUserConnectionRequestDTO()
            { UserGuid = currentUserGuid });

        public async Task<ResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(
            AddUserConnectionRequestDTO addUserConnectionRequestDTO)
            => await connectionService.AddConnectionAsync(addUserConnectionRequestDTO);

        public async Task<ResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>> GetUserConnectionAsync(
            Guid userGuid)
            => await connectionService.GetUserConnection(new GetUserConnectionRequestDTO() { UserGuid = userGuid });

        public async Task<ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>> SendBoardStateToClient(
            BoardStateRequestDTO boardStateConnectionRequestDTO, string player, bool isMyConnection)
            => await connectionService.SendBoardStateToClient(boardStateConnectionRequestDTO, player, isMyConnection);

        public async Task<ResponseDTO<DisconnectedUserNotificationResponseDTO, ChessGameResponseMessage>>
            SendDisconnectedUserNotificationAsync(KeyValuePair<Guid, UserConnectionDTO> userCnnectionDTO)
        {
            var invalidResponse = ResponseDTO<DisconnectedUserNotificationResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(
                    new DisconnectedUserNotificationResponseDTO
                    {
                        IsUserDisconnectedSuccess = false,
                        ActiveGame = default,
                    },
                    ChessGameResponseMessage.InternalServerError,
                    HttpStatusCode.InternalServerError);

            // Notify the opponent that the user has disconnected
            var disconnectedUserResponse = await connectionService.NotifyDisconnectedUser(
                new DisconnectedUserNotificationRequestDTO()
                { ConnectionId = Context.ConnectionId });

            if (!disconnectedUserResponse.IsSuccess)
                return invalidResponse;
            //Remove the user's connection from the database
            var removedUserResponse = await connectionService.RemoveConnectionAsConnectionIdAsync(
                new RemoveUserConnectionRequestDTO()
                { ConnectionId = Context.ConnectionId });

            if (!removedUserResponse.IsSuccess)
                return invalidResponse;

            return ResponseDTO<DisconnectedUserNotificationResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(
                    new DisconnectedUserNotificationResponseDTO
                    {
                        IsUserDisconnectedSuccess = false,
                        ActiveGame = default,
                    },
                    ChessGameResponseMessage.InternalServerError,
                    HttpStatusCode.InternalServerError);
        }

        //connectionService
    }
}