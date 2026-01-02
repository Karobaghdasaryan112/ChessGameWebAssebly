using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Services.HubServices;
using ChessGame.Core.Services.Services.Validations;
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

namespace ChessGame.Infrastructure.Infrastructure.HubServices
{
    public class ConnectionService(BaseHubService baseHubService, GenericValidationService validationService)
        : IConnectionService
    {
        internal BaseHubService _baseHubService = baseHubService;


        public ConcurrentDictionary<Guid, UserConnectionDTO> CurrentConnectionState => ActiveGames._connections;

        public async Task<ResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>>
            GetUserConnection(GetUserConnectionRequestDTO getUserConnectionRequestDTO)
        {
            var validationResult = await validationService.ValidateAsync(getUserConnectionRequestDTO);
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(GetUserConnectionResponseDTO)))!;

            if (!CurrentConnectionState.TryGetValue(getUserConnectionRequestDTO.UserGuid,
                    out var currentUserConnection))
                return await Task.FromResult(
                    ResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        new GetUserConnectionResponseDTO()
                        {
                            UserConnectionDTO = default
                        },
                        ChessGameResponseMessage.PlayerNotFound,
                        HttpStatusCode.NotFound,
                        [$"user connection Not Found for UserId {getUserConnectionRequestDTO.UserGuid}"]));

            return await Task.FromResult(
                ResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    new GetUserConnectionResponseDTO()
                    {
                        UserConnectionDTO = currentUserConnection
                    },
                    ChessGameResponseMessage.UserConnectionFoundSuccess,
                    HttpStatusCode.Found));
        }

        public async Task<ResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>>
            AddConnectionAsync(AddUserConnectionRequestDTO addUserConnectionRequestDTO)
        {
            //Validation
            var validationResult = await validationService.ValidateAsync(addUserConnectionRequestDTO);
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(AddUserConnectionResponseDTO)))!;

            var successResponse =
                ResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    new AddUserConnectionResponseDTO()
                    {
                        IsAdded = true
                    },
                    ChessGameResponseMessage.ConnectionAddedSuccess,
                    HttpStatusCode.Created);


            var existUserResult = await GetUserConnection(
                new GetUserConnectionRequestDTO()
                {
                    UserGuid = addUserConnectionRequestDTO.userGuid
                });

            if (existUserResult.IsSuccess)
            {
                if (existUserResult.Data.UserConnectionDTO.UserName ==
                    addUserConnectionRequestDTO.userConnection.UserName &&
                    existUserResult.Data.UserConnectionDTO.ConnectionId !=
                    addUserConnectionRequestDTO.userConnection.ConnectionId)
                    CurrentConnectionState[addUserConnectionRequestDTO.userGuid].ConnectionId =
                        addUserConnectionRequestDTO.userConnection.ConnectionId;

                return successResponse;
            }

            if (!CurrentConnectionState.TryAdd(addUserConnectionRequestDTO.userGuid,
                    addUserConnectionRequestDTO.userConnection))
                return ResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>
                    .CreateErrorResponse(
                        new AddUserConnectionResponseDTO()
                        {
                            IsAdded = false
                        },
                        ChessGameResponseMessage.InternalServerError,
                        HttpStatusCode.InternalServerError,
                        [$"cannot Added the UserConnection for User {addUserConnectionRequestDTO.userGuid}"]);

            await _baseHubService.SendUsersChange(new KeyValuePair<Guid, UserConnectionDTO>(
                addUserConnectionRequestDTO.userGuid, addUserConnectionRequestDTO.userConnection));

            return successResponse;

        }

        public async Task<ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>>
            RemoveConnectionAsUserGuidAsync(
                RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO)
        {
            var validationResult = await validationService.ValidateAsync(removeUserConnectionRequestDTO);
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(RemoveUserConnectionResponseDTO)))!;

            if (!CurrentConnectionState.TryRemove(removeUserConnectionRequestDTO.UserGuid,
                    out var removedConnection))
                return ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>
                    .CreateErrorResponse(
                        new RemoveUserConnectionResponseDTO()
                        {
                            IsRemoved = false
                        },
                        ChessGameResponseMessage.PlayerNotFound,
                        HttpStatusCode.NotFound,
                        [$"cannot Delete the UserConnection for User {removeUserConnectionRequestDTO.UserGuid}"]);

            return ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(
                    new RemoveUserConnectionResponseDTO() { IsRemoved = true },
                    ChessGameResponseMessage.UserConnectionRemovedSuccess,
                    HttpStatusCode.Found);

        }

        public async Task<ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>>
            RemoveConnectionAsConnectionIdAsync(RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO)
        {
            var validationResult = await validationService.ValidateAsync(removeUserConnectionRequestDTO);
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(RemoveUserConnectionResponseDTO)))!;

            var errorResponse =
                ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new RemoveUserConnectionResponseDTO()
                    {
                        IsRemoved = false
                    },
                    ChessGameResponseMessage.UserConnectionNotFound,
                    HttpStatusCode.NotFound,
                    [
                        $"cannot Delete the UserConnection ConnectionId-{removeUserConnectionRequestDTO.ConnectionId}"
                    ]);

            var removeConnection = CurrentConnectionState.FirstOrDefault(connectionKvp =>
                connectionKvp.Value.ConnectionId == removeUserConnectionRequestDTO.ConnectionId);

            if (removeConnection.Equals(null))
                return errorResponse;

            if (!CurrentConnectionState.TryRemove(removeConnection))
                return errorResponse;


            return ResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(
                    new RemoveUserConnectionResponseDTO()
                    {
                        IsRemoved = true
                    },
                    ChessGameResponseMessage.UserConnectionRemovedSuccess,
                    HttpStatusCode.Found);

        }

        public async Task<ResponseDTO<RemoveUserFromGameResponseDTO, ChessGameResponseMessage>>
            RemoveUsersFromGameAsync(RemoveUserFromGameRequestDTO removeUserFromGameRequestDTO)
        {
            var validationResult = await validationService.ValidateAsync(removeUserFromGameRequestDTO);
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(RemoveUserFromGameResponseDTO)))!;

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


            return ResponseDTO<RemoveUserFromGameResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new RemoveUserFromGameResponseDTO()
                {
                    IsRemoved = true
                },
                ChessGameResponseMessage.UsersRemovedFromGameSuccess,
                HttpStatusCode.OK);
        }

        public async Task<ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>>
            SendBoardStateToClient(BoardStateRequestDTO boardStateConnectionRequestDTO,
                string player, bool isMyConnection, bool win = false)
        {
            //Validation
            var validationResult = await validationService.ValidateAsync(boardStateConnectionRequestDTO);
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(BoardStateResponseDTO)))!;



            string selectedGameOpponentConnectionId = string.Empty;
            if (boardStateConnectionRequestDTO.IsOpponentComputer)
            {
                selectedGameOpponentConnectionId = CurrentConnectionState
                    .FirstOrDefault(connection => connection.Value.UserName == player).Value.ConnectionId!;
            }
            else
            {
                var selectedGameKeyValue = CurrentConnectionState.Where(gameIdUserConnection =>
                    gameIdUserConnection.Value?.GameId ==
                    boardStateConnectionRequestDTO.GameId)
                .Select(selectedGameUserConnection => selectedGameUserConnection.Value).ToList();


                selectedGameKeyValue = isMyConnection
                    ? selectedGameKeyValue.Where(keyValue => keyValue.UserName == player).ToList()
                    : selectedGameKeyValue.Where(keyValue => keyValue.UserName != player).ToList();


                if (selectedGameKeyValue == null)
                    return ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        new BoardStateResponseDTO()
                        {
                            GameId = boardStateConnectionRequestDTO.GameId,
                            Player = boardStateConnectionRequestDTO.Player
                        },
                        ChessGameResponseMessage.InvalidMove,
                        System.Net.HttpStatusCode.BadRequest);

                selectedGameOpponentConnectionId = selectedGameKeyValue.First().ConnectionId;
            }

            var boardStateResposneDTO = new BoardStateResponseDTO()
            {
                GameId = boardStateConnectionRequestDTO.GameId,
                CutableFigure = null!,
                IsReadyToEvent = boardStateConnectionRequestDTO.IsReadyToEvent,
                From = boardStateConnectionRequestDTO.From,
                To = boardStateConnectionRequestDTO.To,
                OpponentConnectionId = selectedGameOpponentConnectionId,
                KingPosition = boardStateConnectionRequestDTO.CheckedKingPosition,
                IsKingChecked = boardStateConnectionRequestDTO.IsKingChecked,
                IsKingMate = boardStateConnectionRequestDTO.IsKingMate,
                OpponentColor = boardStateConnectionRequestDTO.OpponentColor,
                IsMyConnection = isMyConnection,
                Win = win
            };

            var sendBoardResposneDTO =
                ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    boardStateResposneDTO, ChessGameResponseMessage.Draw);

            await _baseHubService.ReceiveBoardUpdateAsync(sendBoardResposneDTO);

            return ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new BoardStateResponseDTO()
                {
                    GameId = boardStateConnectionRequestDTO.GameId,
                    Player = boardStateConnectionRequestDTO.Player
                },
                ChessGameResponseMessage.MoveSuccessful,
                System.Net.HttpStatusCode.OK);
        }
    }
}
