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
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.ChessGameResource.Enums.Users;

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
        //-------------------------------------------------------------------------
        public override async Task OnConnectedAsync()
            => await base.OnConnectedAsync();

        //-------------------------------------------------------------------------

        //-------------------------------------------------------------------------
        public override async Task OnDisconnectedAsync(Exception? exception)
            => await base.OnDisconnectedAsync(exception);

        //-------------------------------------------------------------------------


        //INVITATION-SERVICE
        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<SendInvitationsResponseDTO>> SendInviteAsync(
            PipeLineRequest<SendInvitationRequestDTO> connectionRequestDto)
            => await pipeLineHelper.Execute(
                connectionRequestDto.Request,
                Context,
                async () => await invitationService.SendInviteAsync(connectionRequestDto.Request));
        //-------------------------------------------------------------------------

        //-------------------------------------------------------------------------
        public Task Ping() => Task.CompletedTask;
        //-------------------------------------------------------------------------

        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<AcceptInvitationResponseDTO>> AcceptInviteAsync(
            PipeLineRequest<AcceptInvitationRequestDTO> acceptInvitationRequestDto)
            => await pipeLineHelper.Execute<AcceptInvitationRequestDTO, AcceptInvitationResponseDTO>(
                acceptInvitationRequestDto.Request,
                Context,
                async () => await invitationService.AcceptInviteAsync(
                    new AcceptInvitationRequestDTO()
                    {
                        PlayEvent = acceptInvitationRequestDto.Request.PlayEvent,
                        inviterUserGuid = acceptInvitationRequestDto.Request.inviterUserGuid,
                        receiverUserGuid = acceptInvitationRequestDto.Request.receiverUserGuid
                    }));

        //-------------------------------------------------------------------------

        //-------------------------------------------------------------------------
        public async Task CancelInviteAsync(Guid inviterUserGuid, Guid receiverUserGuid)
            => await invitationService.CancelInviteAsync(inviterUserGuid, receiverUserGuid);
        //-------------------------------------------------------------------------
        //INVITATION-SERVICE


        //GAME-SERVICE
        //-------------------------------------------------------------------------
        public async Task ClearGameAsync(Guid gameId)
            => await gameService.ClearGameAsync(gameId);
        //-------------------------------------------------------------------------


        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<SendGameStateResponseDTO>> SendGameStateAsync(
            PipeLineRequest<SendGameStateReqeustDTO> gameStateReqeustDTO)
            => await pipeLineHelper.Execute(
                gameStateReqeustDTO.Request,
                Context,
                async () => await gameService.SendGameStateAsync(gameStateReqeustDTO));
        //-------------------------------------------------------------------------


        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<GetOnlinePlayersResponseDTO>> GetOnlinePlayersAsync(
            PipeLineRequest<GetONlinePlayersRequestDTO> connectionRequestDTO)
            => await pipeLineHelper.Execute<GetONlinePlayersRequestDTO, GetOnlinePlayersResponseDTO>(
                connectionRequestDTO.Request,
                Context,
                async () => await gameService.GetOnlinePlayersAsync(connectionRequestDTO));
        //-------------------------------------------------------------------------


        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<MoveResponseDTO>> SendMoveAsync(
            PipeLineRequest<MoveRequestDTO> sendMoveConnectionRequestDTO)
            => await pipeLineHelper.Execute<MoveRequestDTO, MoveResponseDTO>(sendMoveConnectionRequestDTO.Request,
                Context,
                async () => await gameService.SendMoveAsync(sendMoveConnectionRequestDTO));
        //-------------------------------------------------------------------------


        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<SameFigureResposneDTO>> SendIsSameFigureClickedAsync(
            PipeLineRequest<SameFigureRequest> sameFigureRequest)
            => await pipeLineHelper.Execute(
                sameFigureRequest.Request,
                Context,
                async () => await gameService.SendIsSameFigureClickedAsync(sameFigureRequest));
        //-------------------------------------------------------------------------


        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<ClickResponseDTO>> SendClickAsync(
            PipeLineRequest<ClickRequestDTO> sendClickConnectionRequestDTO)
            => await pipeLineHelper.Execute(
                sendClickConnectionRequestDTO.Request,
                Context,
                async () => await gameService.SendClickAsync(sendClickConnectionRequestDTO));
        //-------------------------------------------------------------------------


        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<TrainingGameResponseDTO>> RequestTrainingGameAsync(
            PipeLineRequest<TrainingGameRequestDTO> trainingGameRequestDTO)
            => await pipeLineHelper.Execute(
                trainingGameRequestDTO.Request,
                Context,
                async () => await gameService.RequestTrainingGameAsync(trainingGameRequestDTO));
        //-------------------------------------------------------------------------

        //GAME-SERVICE


        //CONNECTION-SERVICE
        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<RemoveUserConnectionResponseDTO>> RemoveConnectionAsync(
            PipeLineRequest<RemoveUserConnectionRequestDTO> requestDTO)
            => await pipeLineHelper.Execute(
                requestDTO.Request,
                Context,
                async () => await connectionService.RemoveConnectionAsUserGuidAsync(
                    new RemoveUserConnectionRequestDTO()
                    {
                        UserGuid = requestDTO.Request.UserGuid
                    }));
        //-------------------------------------------------------------------------


        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<AddUserConnectionResponseDTO>> AddConnectionAsync(
            PipeLineRequest<AddUserConnectionRequestDTO> addUserConnectionRequestDTO)
            => await pipeLineHelper.Execute(
                addUserConnectionRequestDTO.Request,
                Context,
                async () => await connectionService.AddConnectionAsync(addUserConnectionRequestDTO.Request));
        //-------------------------------------------------------------------------


        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<GetUserConnectionResponseDTO>> GetUserConnectionAsync(
            PipeLineRequest<GetUserConnectionRequestDTO> connectionRequestDTO)
            => await pipeLineHelper.Execute(
                connectionRequestDTO.Request,
                Context,
                async () => await connectionService.GetUserConnection(connectionRequestDTO.Request));
        //-------------------------------------------------------------------------


        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<BoardStateSenderResponseDTO>> SendBoardStateToClient(
            PipeLineRequest<BoardStateSenderRequestDTO> sendGameStateReqeustDTO)
            => await connectionService.SendBoardStateToClient(sendGameStateReqeustDTO.Request);
        //-------------------------------------------------------------------------


        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<DisconnectedUserNotificationResponseDTO>>
            SendDisconnectedUserNotificationAsync(KeyValuePair<Guid, UserConnectionDTO> userCnnectionDTO)
        {
            var invalidResponse =
                new PipeLineResponse<DisconnectedUserNotificationResponseDTO>()
                {
                    Response = ResponseDTO<DisconnectedUserNotificationResponseDTO, ChessGameResponseMessage>
                        .CreateSuccessResponse(
                            new DisconnectedUserNotificationResponseDTO
                            {
                                IsUserDisconnectedSuccess = false,
                                ActiveGame = default,
                            },
                            ChessGameResponseMessage.InternalServerError,
                            HttpStatusCode.InternalServerError)
                };

            // Notify the opponent that the user has disconnected
            var disconnectedUserResponse = await connectionService.NotifyDisconnectedUser(
                new DisconnectedUserNotificationRequestDTO()
                    { ConnectionId = Context.ConnectionId });

            var response = disconnectedUserResponse.Response;

            if (!response.IsSuccess)
                return invalidResponse;
            //Remove the user's connection from the database
            var removedUserResponse = await connectionService.RemoveConnectionAsConnectionIdAsync(
                new RemoveUserConnectionRequestDTO()
                    { ConnectionId = Context.ConnectionId });

            if (!response.IsSuccess)
                return invalidResponse;

            return
                new PipeLineResponse<DisconnectedUserNotificationResponseDTO>()
                {
                    Response =
                        ResponseDTO<DisconnectedUserNotificationResponseDTO, ChessGameResponseMessage>
                            .CreateSuccessResponse(
                                new DisconnectedUserNotificationResponseDTO
                                {
                                    IsUserDisconnectedSuccess = false,
                                    ActiveGame = null,
                                },
                                ChessGameResponseMessage.InternalServerError,
                                HttpStatusCode.InternalServerError)
                };
        }
        //-------------------------------------------------------------------------


        //-------------------------------------------------------------------------
        public async Task<PipeLineResponse<RemoveUserFromGameResponseDTO>> LeaveGameAsync(
            PipeLineRequest<RemoveUsersFromGameReqeustDTO> leavingPlayerRequestDTO)
        {
            var invalidResponse = new PipeLineResponse<RemoveUserFromGameResponseDTO>()
            {
                Response = ResponseDTO<RemoveUserFromGameResponseDTO, ChessGameResponseMessage>
                    .CreateErrorResponse(
                        new RemoveUserFromGameResponseDTO { IsRemoved = false },
                        ChessGameResponseMessage.InvalidData,
                        HttpStatusCode.BadRequest)
            };

            var leavePlayerGuid = leavingPlayerRequestDTO.Request.CurerntPlayerGuid;
            var gameId = leavingPlayerRequestDTO.Request.GameId;

            if (!connectionService.CurrentConnectionState.TryGetValue(leavePlayerGuid,
                    out var leavingPlayerConnection) || leavingPlayerConnection.Gameinfo == null)
                return
                    invalidResponse;

            var opponentPlayerGuid = leavingPlayerConnection.Gameinfo.Players.Key == leavePlayerGuid
                ? leavingPlayerConnection.Gameinfo.Players.Value
                : leavingPlayerConnection.Gameinfo.Players.Key;

            if (!connectionService.CurrentConnectionState.TryGetValue(opponentPlayerGuid, out var opponentConnection))
            {
                opponentConnection = connectionService.CurrentConnectionState
                    .Where(connection => connection.Key != leavePlayerGuid && connection.Value.GameId == gameId)
                    .Select(connection => connection.Value)
                    .FirstOrDefault();

                if (opponentConnection == null)
                    return invalidResponse;
            }

            if (leavingPlayerRequestDTO.Request.IsLeaveWebSite)
                connectionService.CurrentConnectionState.TryRemove(leavePlayerGuid, out _);
            opponentConnection.Gameinfo = null;
            leavingPlayerConnection.Gameinfo = null;
            leavingPlayerConnection.GameId = default!;

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


            await baseHubService.NotifyOpponentLeftWinAsync(opponentConnection.ConnectionId,
                leavingPlayerConnection.UserName);

            await baseHubService.ForceNavigateToDashboardAsync(leavingPlayerConnection.ConnectionId);

            ActiveGames.ActiveGamesAndBoards.TryRemove(gameId, out _);

            await baseHubService.SendUsersChange(
                new KeyValuePair<Guid, UserConnectionDTO>(opponentPlayerGuid, opponentConnection),
                OnlinePlayerChangeType.Removed);

            return await connectionService.RemoveUsersFromGameAsync(new RemoveUserFromGameRequestDTO
            {
                GameId = gameId
            });
        }
        //-------------------------------------------------------------------------
        //CONNECTION-SERVICE
    }
}