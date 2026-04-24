using ChessGame.Core.Services.Contracts.Hub;
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
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.PipeLine.PipeLineHelper;
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

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        //InvitationService 

        public async Task<PipeLineResponse<SendInvitationsResponseDTO>> SendInviteAsync(PipeLineRequest<SendInvitationRequestDTO> connectionRequestDto)
            => await pipeLineHelper.Execute(
                connectionRequestDto.Request,
                Context,
                async () => await invitationService.SendInviteAsync(connectionRequestDto.Request));


        public async Task<PipeLineResponse<AcceptInvitationResponseDTO>> AcceptInviteAsync(
            AcceptInvitationRequestDTO acceptInvitationRequestDto)
            => await pipeLineHelper.Execute<AcceptInvitationRequestDTO, AcceptInvitationResponseDTO>(
                acceptInvitationRequestDto,
                Context, async () =>
                    await invitationService.AcceptInviteAsync(new AcceptInvitationRequestDTO()
                    {
                        inviterUserGuid = acceptInvitationRequestDto.inviterUserGuid,
                        receiverUserGuid = acceptInvitationRequestDto.receiverUserGuid
                    }));


        public async Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
            => await invitationService.CancelInviteAsync(inviterUserGuid, receiverUserGuid);

        //InvitationService 


        //GameService

        public async Task ClearGameAsync(Guid gameId)
            => await gameService.ClearGameAsync(gameId);

        public async Task<ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(
            SendGameStateReqeustDTO gameStateReqeustDTO)
            => await gameService.SendGameStateAsync(gameStateReqeustDTO);

        public async Task<PipeLineResponse<GetOnlinePlayersResponseDTO>> GetOnlinePlayersAsync(
            PipeLineRequest<GetONlinePlayersRequestDTO> connectionRequestDTO)
        {
           var result = await pipeLineHelper.Execute<GetONlinePlayersRequestDTO, GetOnlinePlayersResponseDTO>(
                connectionRequestDTO.Request,
                Context, async () =>
                    await gameService.GetOnlinePlayersAsync(connectionRequestDTO));

           return result;
        }

        public async Task<PipeLineResponse<MoveResponseDTO>> SendMoveAsync(
            MoveRequestDTO sendMoveConnectionRequestDTO)

            => await pipeLineHelper.Execute<MoveRequestDTO, MoveResponseDTO>(sendMoveConnectionRequestDTO, Context, async () => await gameService.SendMoveAsync(sendMoveConnectionRequestDTO));

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