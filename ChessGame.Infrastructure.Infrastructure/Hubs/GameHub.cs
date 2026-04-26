using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Extentions;
using ChessGame.Core.Services.Services.HubServices;
using Microsoft.AspNetCore.SignalR;
using SharedResources.ChessGameResource.StaticResources;
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
using SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests;
using SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;
using SharedResources.Validation.ChessGameValidations.ResponseValidations.ConnectionResponses;
using System.Net;
using System.Linq;

namespace ChessGame.Infrastructure.Infrastructure.Hubs
{
    public class GameHub(
        BaseHubService baseHubService,
        IInvitationService invitationService,
        IGameService gameService,
        IConnectionService connectionService,
        IBoardService boardService,
        PipeLineExecutionHelper pipeLineHelper)
        : Hub
    {
        public override async Task OnConnectedAsync()
            => await base.OnConnectedAsync();

        public override async Task OnDisconnectedAsync(Exception? exception)
            => await base.OnDisconnectedAsync(exception);

        //InvitationService 

        public async Task<PipeLineResponse<SendInvitationsResponseDTO>> SendInviteAsync(
            PipeLineRequest<SendInvitationRequestDTO> connectionRequestDto)
            => await pipeLineHelper.Execute(
                connectionRequestDto.Request,
                Context,
                async () => await invitationService.SendInviteAsync(connectionRequestDto.Request));

        //To keep the connection alive
        //Send ping into server
        //receive and return Completed Task
        public Task Ping() => Task.CompletedTask;
        
        public async Task<PipeLineResponse<AcceptInvitationResponseDTO>> AcceptInviteAsync(
            PipeLineRequest<AcceptInvitationRequestDTO> acceptInvitationRequestDto)
            => await pipeLineHelper.Execute<AcceptInvitationRequestDTO, AcceptInvitationResponseDTO>(
                acceptInvitationRequestDto.Request,
                Context, async () =>
                    await invitationService.AcceptInviteAsync(new AcceptInvitationRequestDTO()
                    {
                        inviterUserGuid = acceptInvitationRequestDto.Request.inviterUserGuid,
                        receiverUserGuid = acceptInvitationRequestDto.Request.receiverUserGuid
                    }));


        public async Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
            => await invitationService.CancelInviteAsync(inviterUserGuid, receiverUserGuid);

        //InvitationService 


        //GameService

        public async Task ClearGameAsync(Guid gameId)
            => await gameService.ClearGameAsync(gameId);

        public async Task<PipeLineResponse<SendGameStateResponseDTO>> SendGameStateAsync(
            PipeLineRequest<SendGameStateReqeustDTO> gameStateReqeustDTO)
            => await pipeLineHelper.Execute(gameStateReqeustDTO.Request, Context, async () =>
                await gameService.SendGameStateAsync(gameStateReqeustDTO));

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
            PipeLineRequest<MoveRequestDTO> sendMoveConnectionRequestDTO)
            => await pipeLineHelper.Execute<MoveRequestDTO, MoveResponseDTO>(sendMoveConnectionRequestDTO.Request,
                Context,
                async () => await gameService.SendMoveAsync(sendMoveConnectionRequestDTO));

        public async Task<PipeLineResponse<SameFigureResposneDTO>> SendIsSameFigureClickedAsync(
            PipeLineRequest<SameFigureRequest> sameFigureRequest)
            => await pipeLineHelper.Execute(
                sameFigureRequest.Request,
                Context,
                async () => await gameService.SendIsSameFigureClickedAsync(sameFigureRequest));


        public async Task<PipeLineResponse<ClickResponseDTO>> SendClickAsync(
            PipeLineRequest<ClickRequestDTO> sendClickConnectionRequestDTO)
            => await pipeLineHelper.Execute(sendClickConnectionRequestDTO.Request, Context,
                async () => await gameService.SendClickAsync(sendClickConnectionRequestDTO));

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

        public async Task<ResponseDTO<RemoveUserFromGameResponseDTO, ChessGameResponseMessage>> LeaveGameAsync(
            Guid gameId, Guid leavingPlayerGuid)
        {
            if (!connectionService.CurrentConnectionState.TryGetValue(leavingPlayerGuid, out var leavingPlayerConnection))
            {
                return ResponseDTO<RemoveUserFromGameResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new RemoveUserFromGameResponseDTO { IsRemoved = false },
                    ChessGameResponseMessage.PlayerNotFound,
                    HttpStatusCode.NotFound);
            }

            if (leavingPlayerConnection.Gameinfo == null)
            {
                return ResponseDTO<RemoveUserFromGameResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new RemoveUserFromGameResponseDTO { IsRemoved = false },
                    ChessGameResponseMessage.InvalidData,
                    HttpStatusCode.BadRequest);
            }

            var opponentPlayerGuid = leavingPlayerConnection.Gameinfo.Players.Key == leavingPlayerGuid
                ? leavingPlayerConnection.Gameinfo.Players.Value
                : leavingPlayerConnection.Gameinfo.Players.Key;

            if (!connectionService.CurrentConnectionState.TryGetValue(opponentPlayerGuid, out var opponentConnection))
            {
                opponentConnection = connectionService.CurrentConnectionState
                    .Where(connection => connection.Key != leavingPlayerGuid && connection.Value.GameId == gameId)
                    .Select(connection => connection.Value)
                    .FirstOrDefault();

                if (opponentConnection == null)
                {
                    return ResponseDTO<RemoveUserFromGameResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        new RemoveUserFromGameResponseDTO { IsRemoved = false },
                        ChessGameResponseMessage.PlayerNotFound,
                        HttpStatusCode.NotFound);
                }
            }

            if (ActiveGames.ActiveGamesAndBoards.TryGetValue(gameId, out var boardState))
            {
                await boardService.SavePositionsAsync(new SavePositionsRequestDTO
                {
                    GameId = gameId,
                    FEN = boardState.FromBoardToFen()
                });
            }

            await boardService.SaveGameEventAndWinnerAsync(new SaveGameEventAndWinnerRequestDTO
            {
                GameId = gameId,
                WinnerPlayerGuid = opponentPlayerGuid
            });

            await baseHubService.NotifyOpponentUserDisconnected(new KeyValuePair<Guid, UserConnectionDTO>(
                leavingPlayerGuid,
                leavingPlayerConnection));
            await baseHubService.NotifyOpponentLeftWinAsync(opponentConnection.ConnectionId, leavingPlayerConnection.UserName);
            await baseHubService.ForceNavigateToDashboardAsync(leavingPlayerConnection.ConnectionId);

            ActiveGames.ActiveGamesAndBoards.TryRemove(gameId, out _);

            return await connectionService.RemoveUsersFromGameAsync(new RemoveUserFromGameRequestDTO
            {
                GameId = gameId
            });
        }

        //connectionService
    }
}