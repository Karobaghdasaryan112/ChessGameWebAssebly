using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Services.HubServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests;
using SharedResources.Validation.ChessGameValidations.ResponseValidations.ConnectionResponses;
using System.Collections.Concurrent;
using System.Net;
using SharedResources.ChessGameResource.Enums.Users;
using SharedResources.PipeLine.PipeLineContext;

namespace ChessGame.Infrastructure.Infrastructure.HubServices
{
    public class ConnectionService(
        BaseHubService baseHubService,
        ILogger<ConnectionService> logger)
        : IConnectionService
    {
        private BaseHubService _baseHubService = baseHubService;

        public ConcurrentDictionary<Guid, UserConnectionDTO> CurrentConnectionState => ActiveGames._connections;


        //---------------------------------------------------------------------------------------------------------
        public async Task<PipeLineResponse<GetUserConnectionResponseDTO>>
            GetUserConnection(GetUserConnectionRequestDTO getUserConnectionRequestDTO)
        {
            if (!CurrentConnectionState.TryGetValue(getUserConnectionRequestDTO.UserGuid,
                    out var currentUserConnection))
                return await Task.FromResult(
                    new PipeLineResponse<GetUserConnectionResponseDTO>()
                    {
                        Response = ResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>
                            .CreateErrorResponse(
                                new GetUserConnectionResponseDTO()
                                {
                                    UserConnectionDTO = default
                                },
                                ChessGameResponseMessage.PlayerNotFound,
                                HttpStatusCode.NotFound,
                                [$"user connection Not Found for UserId {getUserConnectionRequestDTO.UserGuid}"])
                    });

            return await Task.FromResult(
                new PipeLineResponse<GetUserConnectionResponseDTO>()
                {
                    Response =
                        ResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                            new GetUserConnectionResponseDTO()
                            {
                                UserConnectionDTO = currentUserConnection
                            },
                            ChessGameResponseMessage.UserConnectionFoundSuccess,
                            HttpStatusCode.Found)
                });
        }
        //---------------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------------------
        public async Task<PipeLineResponse<AddUserConnectionResponseDTO>>
            AddConnectionAsync(AddUserConnectionRequestDTO addUserConnectionRequestDTO)
        {
            var successResponse =
                new PipeLineResponse<AddUserConnectionResponseDTO>()
                {
                    Response =
                        ResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                            new AddUserConnectionResponseDTO()
                            {
                                IsAdded = true
                            },
                            ChessGameResponseMessage.ConnectionAddedSuccess,
                            HttpStatusCode.Created)
                };

            var existUserResult = await GetUserConnection(
                new GetUserConnectionRequestDTO()
                {
                    UserGuid = addUserConnectionRequestDTO.userGuid
                });

            var getUserConnectionResponse = existUserResult.Response;

            if (getUserConnectionResponse.IsSuccess)
            {
                if (getUserConnectionResponse.Data.UserConnectionDTO.UserName !=
                    addUserConnectionRequestDTO.userConnection.UserName ||
                    getUserConnectionResponse.Data.UserConnectionDTO.ConnectionId ==
                    addUserConnectionRequestDTO.userConnection.ConnectionId) return successResponse;

                await _baseHubService.SendUsersChange(new KeyValuePair<Guid, UserConnectionDTO>(
                        addUserConnectionRequestDTO.userGuid, addUserConnectionRequestDTO.userConnection),
                    OnlinePlayerChangeType.Reconnected);
                CurrentConnectionState[addUserConnectionRequestDTO.userGuid].ConnectionId =
                    addUserConnectionRequestDTO.userConnection.ConnectionId;

                return successResponse;
            }

            if (!CurrentConnectionState.TryAdd(addUserConnectionRequestDTO.userGuid,
                    addUserConnectionRequestDTO.userConnection))
                return
                    new PipeLineResponse<AddUserConnectionResponseDTO>()
                    {
                        Response = ResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>
                            .CreateErrorResponse(
                                new AddUserConnectionResponseDTO()
                                {
                                    IsAdded = false
                                },
                                ChessGameResponseMessage.InternalServerError,
                                HttpStatusCode.InternalServerError,
                                [$"cannot Added the UserConnection for User {addUserConnectionRequestDTO.userGuid}"])
                    };

            await _baseHubService.SendUsersChange(new KeyValuePair<Guid, UserConnectionDTO>(
                    addUserConnectionRequestDTO.userGuid, addUserConnectionRequestDTO.userConnection),
                OnlinePlayerChangeType.Added);

            return successResponse;
        }
        //---------------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------------------
        public async Task<PipeLineResponse<RemoveUserConnectionResponseDTO>>
            RemoveConnectionAsUserGuidAsync(
                RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO)
        {
            if (!CurrentConnectionState.TryRemove(removeUserConnectionRequestDTO.UserGuid,
                    out var removedConnection))
                return
                    new PipeLineResponse<RemoveUserConnectionResponseDTO>
                    {
                        Response = ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>
                            .CreateErrorResponse(
                                new RemoveUserConnectionResponseDTO()
                                {
                                    IsRemoved = false
                                },
                                ChessGameResponseMessage.PlayerNotFound,
                                HttpStatusCode.NotFound,
                                [
                                    $"cannot Delete the UserConnection for User {removeUserConnectionRequestDTO.UserGuid}"
                                ])
                    };


            await _baseHubService._hubContext.Clients.All.SendAsync(
                "ReceiveUpdatedUsers",
                OnlinePlayerChangeType.Removed,
                new KeyValuePair<Guid, UserConnectionDTO>(removeUserConnectionRequestDTO.UserGuid, removedConnection));


            return new PipeLineResponse<RemoveUserConnectionResponseDTO>
            {
                Response = ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>
                    .CreateSuccessResponse(
                        new RemoveUserConnectionResponseDTO() { IsRemoved = true },
                        ChessGameResponseMessage.UserConnectionRemovedSuccess,
                        HttpStatusCode.Found)
            };
        }
        //---------------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------------------
        public async Task<PipeLineResponse<RemoveUserConnectionResponseDTO>>
            RemoveConnectionAsConnectionIdAsync(RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO)
        {
            var errorResponse =
                new PipeLineResponse<RemoveUserConnectionResponseDTO>()
                {
                    Response = ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>
                        .CreateErrorResponse(
                            new RemoveUserConnectionResponseDTO()
                            {
                                IsRemoved = false
                            },
                            ChessGameResponseMessage.UserConnectionNotFound,
                            HttpStatusCode.NotFound,
                            [
                                $"cannot Delete the UserConnection ConnectionId-{removeUserConnectionRequestDTO.ConnectionId}"
                            ])
                };


            var removeConnection = CurrentConnectionState.FirstOrDefault(connectionKvp =>
                connectionKvp.Value.ConnectionId == removeUserConnectionRequestDTO.ConnectionId);

            if (removeConnection.Equals(default(KeyValuePair<Guid, UserConnectionDTO>)) ||
                !CurrentConnectionState.TryRemove(removeConnection))
                return errorResponse;

            await _baseHubService._hubContext.Clients.All.SendAsync(
                "RemovedUserChangeNotification",
                removeConnection);

            return
                new PipeLineResponse<RemoveUserConnectionResponseDTO>()
                {
                    Response =
                        ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>
                            .CreateSuccessResponse(
                                new RemoveUserConnectionResponseDTO()
                                {
                                    IsRemoved = true
                                },
                                ChessGameResponseMessage.UserConnectionRemovedSuccess,
                                HttpStatusCode.Found)
                };
        }
        //---------------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------------------
        public async Task<PipeLineResponse<DisconnectedUserNotificationResponseDTO>>
            NotifyDisconnectedUser(DisconnectedUserNotificationRequestDTO disconnectedUserNotificationRequestDTO)
        {
            var currentConnection = CurrentConnectionState.FirstOrDefault(connection =>
                connection.Value.ConnectionId == disconnectedUserNotificationRequestDTO.ConnectionId);

            var opponentForCurrentConnection = CurrentConnectionState
                .FirstOrDefault(aliveConnection =>
                    aliveConnection.Value.GameId == currentConnection.Value.GameId &&
                    (aliveConnection.Value.Gameinfo?.Players.Value == currentConnection.Key ||
                     aliveConnection.Value.Gameinfo?.Players.Key == currentConnection.Key ||
                     currentConnection.Key == default));


            if (opponentForCurrentConnection.Key != default && opponentForCurrentConnection.Value != default)
            {
                logger.LogInformation(
                    $"Opponent found for the disconnected user with ConnectionId: {disconnectedUserNotificationRequestDTO.ConnectionId}. Opponent ConnectionId: {opponentForCurrentConnection.Value?.ConnectionId}");


                await _baseHubService.NotifyOpponentUserDisconnected(opponentForCurrentConnection);

                return
                    new PipeLineResponse<DisconnectedUserNotificationResponseDTO>()
                    {
                        Response =
                            ResponseDTO<DisconnectedUserNotificationResponseDTO, ChessGameResponseMessage>
                                .CreateSuccessResponse(
                                    new DisconnectedUserNotificationResponseDTO()
                                    {
                                        IsUserDisconnectedSuccess = true,
                                        ActiveGame = opponentForCurrentConnection.Value
                                    },
                                    ChessGameResponseMessage.SuccessUserConnections,
                                    HttpStatusCode.OK)
                    };
            }

            logger.LogInformation(
                $"No opponent found for the disconnected user with ConnectionId: {disconnectedUserNotificationRequestDTO.ConnectionId}");

            return new PipeLineResponse<DisconnectedUserNotificationResponseDTO>()
            {
                Response =
                    ResponseDTO<DisconnectedUserNotificationResponseDTO, ChessGameResponseMessage>
                        .CreateSuccessResponse(
                            new DisconnectedUserNotificationResponseDTO()
                            {
                                IsUserDisconnectedSuccess = true,
                                ActiveGame = null!
                            },
                            ChessGameResponseMessage.SuccessUserConnections,
                            HttpStatusCode.OK)
            };
        }
        //---------------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------------------
        public async Task<PipeLineResponse<RemoveUserFromGameResponseDTO>>
            RemoveUsersFromGameAsync(RemoveUserFromGameRequestDTO removeUserFromGameRequestDTO)
        {
            var activeConnections = CurrentConnectionState.Where(connection =>
                connection.Value.GameId == removeUserFromGameRequestDTO.GameId).ToList();

            var connectionIds = activeConnections.Select(connection => connection.Value.ConnectionId!).ToList();


            activeConnections.ForEach(activeConnection =>
            {
                activeConnection.Value.Gameinfo = null!;
                activeConnection.Value.GameId = Guid.Empty;
            });


            await _baseHubService.RemoveFromGroupAsync(removeUserFromGameRequestDTO.GameId.ToString(),
                connectionIds);

            return
                new PipeLineResponse<RemoveUserFromGameResponseDTO>()
                {
                    Response = ResponseDTO<RemoveUserFromGameResponseDTO, ChessGameResponseMessage>
                        .CreateSuccessResponse(
                            new RemoveUserFromGameResponseDTO()
                            {
                                IsRemoved = true
                            },
                            ChessGameResponseMessage.UsersRemovedFromGameSuccess,
                            HttpStatusCode.OK)
                };
        }
        //---------------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------------------
        public async Task<PipeLineResponse<BoardStateSenderResponseDTO>> SendBoardStateToClient(
            BoardStateSenderRequestDTO request)
        {
            var gameId = request.BoardStateRequestDTO.GameId;
            var currentPlayer = request.Player;

            if (request.BoardStateRequestDTO.IsOpponentComputer)
            {
            }

            var participants = ActiveGames._connections.Values
                .Where(c => c.GameId == gameId)
                .ToList();

            if (!participants.Any())
            {
                return CreateErrorPipeLineResponse(gameId, currentPlayer);
            }

            foreach (var participant in participants)
            {
                bool isMe = participant.UserName == currentPlayer;

                var boardStateDto = MapToResponseDTO(request, participant.ConnectionId, isMe);

                var hubResponse = ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>
                    .CreateSuccessResponse(boardStateDto, ChessGameResponseMessage.MoveSuccessful);

                await _baseHubService.ReceiveBoardUpdateAsync(hubResponse);
            }


            return CreateSuccessPipeLineResponse(gameId, currentPlayer);
        }


        private BoardStateResponseDTO MapToResponseDTO(BoardStateSenderRequestDTO req, string connectionId,
            bool isMyConnection)
        {
            return new BoardStateResponseDTO
            {
                GameId = req.BoardStateRequestDTO.GameId,
                From = req.BoardStateRequestDTO.From,
                To = req.BoardStateRequestDTO.To,
                OpponentConnectionId = connectionId,
                KingPosition = req.BoardStateRequestDTO.CheckedKingPosition,
                IsKingChecked = req.BoardStateRequestDTO.IsKingChecked,
                IsKingMate = req.BoardStateRequestDTO.IsKingMate,
                OpponentColor = req.BoardStateRequestDTO.OpponentColor,
                IsReadyToEvent = req.BoardStateRequestDTO.IsReadyToEvent,
                IsMyConnection = isMyConnection,
                Win = isMyConnection,
                CutableFigure = null!
            };
        }

        private PipeLineResponse<BoardStateSenderResponseDTO> CreateSuccessPipeLineResponse(Guid gameId, string player)
        {
            return new PipeLineResponse<BoardStateSenderResponseDTO>
            {
                Response = ResponseDTO<BoardStateSenderResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    new BoardStateSenderResponseDTO
                    {
                        BoardStateResponse = new BoardStateResponseDTO { GameId = gameId, Player = player }
                    },
                    ChessGameResponseMessage.MoveSuccessful,
                    System.Net.HttpStatusCode.OK)
            };
        }

        private PipeLineResponse<BoardStateSenderResponseDTO> CreateErrorPipeLineResponse(Guid gameId, string player)
        {
            return new PipeLineResponse<BoardStateSenderResponseDTO>
            {
                Response = ResponseDTO<BoardStateSenderResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new BoardStateSenderResponseDTO
                    {
                        BoardStateResponse = new BoardStateResponseDTO { GameId = gameId, Player = player }
                    },
                    ChessGameResponseMessage.InvalidMove,
                    System.Net.HttpStatusCode.BadRequest)
            };
        }
        //---------------------------------------------------------------------------------------------------------
    }
}