using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Services.HubServices;
using ChessGame.Core.Services.Services.Validations;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
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
    public class ConnetionService(BaseHubService baseHubService, GenericValidationService validationService) : IConnectionService
    {
        internal BaseHubService _baseHubService = baseHubService;


        public ConcurrentDictionary<Guid, UserConnectionDTO> CurrentConnectionState => ActiveGames._connections;

        public async Task<ConnectionResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>> GetUserConnection(ConnectionRequestDTO<GetUserConnectionRequestDTO> getUserConnectionRequestDTO)
        {
            //Validation
            var validationResult = await validationService.ValidateAsync(getUserConnectionRequestDTO.Data);
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(GetUserConnectionResponseDTO)))!;

            if (!CurrentConnectionState.TryGetValue(getUserConnectionRequestDTO.Data.UserGuid, out var currentUserConnection))
                return await Task.FromResult(ConnectionResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>.
                    CreateErrorResponse(
                    new GetUserConnectionResponseDTO()
                    {
                        UserConnectionDTO = default
                    },
                    ChessGameResponseMessage.PlayerNotFound,
                    HttpStatusCode.NotFound,
                    [$"user connection Not Found for UserId {getUserConnectionRequestDTO.Data.UserGuid}"]));

            return await Task.FromResult(ConnectionResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>.
                  CreateSuccessResponse(
                  new GetUserConnectionResponseDTO()
                  {
                      UserConnectionDTO = currentUserConnection
                  },
                  ChessGameResponseMessage.UserConnectionFoundSuccess,
                  HttpStatusCode.Found));
        }

        public async Task<ConnectionResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(ConnectionRequestDTO<AddUserConnectionRequestDTO> addUserConnectionRequestDTO)
        {
            //Validation
            var validationResult = await validationService.ValidateAsync(addUserConnectionRequestDTO.Data);
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(AddUserConnectionResponseDTO)))!;

            var successResponse =
                ConnectionResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>.
                    CreateSuccessResponse(
                        new AddUserConnectionResponseDTO()
                        {
                            IsAdded = true
                        },
                        ChessGameResponseMessage.ConnectionAddedSuccess,
                        HttpStatusCode.Created);


            var existUserResult = await GetUserConnection(
                new ConnectionRequestDTO<GetUserConnectionRequestDTO>()
                {
                    Data = new GetUserConnectionRequestDTO()
                    {
                        UserGuid = addUserConnectionRequestDTO.Data.userGuid
                    }
                });

            if (existUserResult.IsSuccess)
            {
                if (existUserResult.Data.UserConnectionDTO.UserName ==
                    addUserConnectionRequestDTO.Data.userConnection.UserName &&
                    existUserResult.Data.UserConnectionDTO.ConnectionId !=
                    addUserConnectionRequestDTO.Data.userConnection.ConnectionId)
                    CurrentConnectionState[addUserConnectionRequestDTO.Data.userGuid].ConnectionId = addUserConnectionRequestDTO.Data.userConnection.ConnectionId;

                return successResponse;
            }

            if (!CurrentConnectionState.TryAdd(addUserConnectionRequestDTO.Data.userGuid, addUserConnectionRequestDTO.Data.userConnection))
                return ConnectionResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>.
                  CreateErrorResponse(
                  new AddUserConnectionResponseDTO()
                  {
                      IsAdded = false
                  },
                  ChessGameResponseMessage.InternalServerError,
                  HttpStatusCode.InternalServerError,
                  [$"cannot Added the UserConnection for User {addUserConnectionRequestDTO.Data.userGuid}"]);

            await _baseHubService.SendUsersChange(new KeyValuePair<Guid, UserConnectionDTO>(addUserConnectionRequestDTO.Data.userGuid, addUserConnectionRequestDTO.Data.userConnection));

            return successResponse;

        }

        public async Task<ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsUserGuidAsync(ConnectionRequestDTO<RemoveUserConnectionRequestDTO> removeUserConnectionRequestDTO)
        {
            //Validation
            var validationResult = await validationService.ValidateAsync(removeUserConnectionRequestDTO.Data);
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(RemoveUserConnectionResponseDTO)))!;

            if (!CurrentConnectionState.TryRemove(removeUserConnectionRequestDTO.Data.UserGuid, out var removedConnection))
                return ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>.
                 CreateErrorResponse(
                 new RemoveUserConnectionResponseDTO()
                 {
                     IsRemoved = false
                 },
                 ChessGameResponseMessage.PlayerNotFound,
                 HttpStatusCode.NotFound,
                 [$"cannot Delete the UserConnection for User {removeUserConnectionRequestDTO.Data.UserGuid}"]);

            //TO DO _baseHubService.SendRemoveUser

            return ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>.
                  CreateSuccessResponse(
                  new RemoveUserConnectionResponseDTO() { IsRemoved = true },
                  ChessGameResponseMessage.UserConnectionRemovedSuccess,
                  HttpStatusCode.Found);

        }
        public async Task<ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsConnectionIdAsync(RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO)
        {
            //Validation
            var validationResult = await validationService.ValidateAsync(removeUserConnectionRequestDTO);
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(RemoveUserConnectionResponseDTO)))!;

            var errorResponse =
                ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>.
                CreateErrorResponse(
                new RemoveUserConnectionResponseDTO()
                {
                    IsRemoved = false
                },
                ChessGameResponseMessage.UserConnectionNotFound,
                HttpStatusCode.NotFound,
                [
                    $"cannot Delete the UserConnection ConnectionId-{removeUserConnectionRequestDTO.ConnectionId}"
                ]);

            var removeConnection = CurrentConnectionState.FirstOrDefault(connectionKvp => connectionKvp.Value.ConnectionId == removeUserConnectionRequestDTO.ConnectionId);

            if (removeConnection.Equals(null))
                return errorResponse;

            if (!CurrentConnectionState.TryRemove(removeConnection))
                return errorResponse;


            return ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>.
                  CreateSuccessResponse(
                  new RemoveUserConnectionResponseDTO()
                  {
                      IsRemoved = true
                  },
                  ChessGameResponseMessage.UserConnectionRemovedSuccess,
                  HttpStatusCode.Found);

        }

        public async Task<ConnectionResponseDTO<RemoveUserFromGameResponseDTO, ChessGameResponseMessage>> RemoveUsersFromGameAsync(ConnectionRequestDTO<RemoveUserFromGameRequestDTO> removeUserFromGameRequestDTO)
        {
            //Validation
            var validationResult = await validationService.ValidateAsync(removeUserFromGameRequestDTO.Data);
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(RemoveUserFromGameResponseDTO)))!;

            //Get Active Connections in Game
            var activeConnections = CurrentConnectionState.Where(connection =>
                connection.Value.GameId == removeUserFromGameRequestDTO.Data.GameId).ToList();

            var connectionIds = activeConnections.Select(connection => connection.Value.ConnectionId!).ToList();

            //Reset Game Info
            activeConnections.ForEach(activeConnection =>
            {
                activeConnection.Value.Gameinfo = null!;
                activeConnection.Value.GameId = Guid.Empty;
            });

            //Remove from Group
            await _baseHubService.RemoveFromGroupAsync(removeUserFromGameRequestDTO.Data.GameId.ToString(), connectionIds);


            //Return Response
            return ConnectionResponseDTO<RemoveUserFromGameResponseDTO, ChessGameResponseMessage>.
                  CreateSuccessResponse(
                  new RemoveUserFromGameResponseDTO()
                  {
                      IsRemoved = true
                  },
                  ChessGameResponseMessage.UsersRemovedFromGameSuccess,
                  HttpStatusCode.OK);
        }

        public async Task<ConnectionResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>> SendBoardStateToClient(ConnectionRequestDTO<BoardStateRequestDTO> boardStateConnectionRequestDTO, string player, bool isMyConnection, bool win = false)
        {
            //Validation
            var validationResult = await validationService.ValidateAsync(boardStateConnectionRequestDTO.Data);
            if (!validationResult.IsValid)
                return (await validationResult.ReturnValidationResult(default(BoardStateResponseDTO)))!;

            var selectedGameKeyValue = CurrentConnectionState.
           Where(gameIdUserConnection =>
               gameIdUserConnection.Value?.GameId ==
               boardStateConnectionRequestDTO.Data.GameId).
           Select(selectedGameUserConnection => selectedGameUserConnection.Value).ToList();


            selectedGameKeyValue = isMyConnection ? selectedGameKeyValue.Where(keyValue => keyValue.UserName == player).ToList()
                : selectedGameKeyValue.Where(keyValue => keyValue.UserName != player).ToList();


            if (selectedGameKeyValue == null)
                return ConnectionResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new BoardStateResponseDTO()
                    {
                        GameId = boardStateConnectionRequestDTO.Data.GameId,
                        Player = boardStateConnectionRequestDTO.Data.Player
                    },
                    ChessGameResponseMessage.InvalidMove,
                    System.Net.HttpStatusCode.BadRequest);

            var selectedGameOpponentConnectionId = selectedGameKeyValue.First().ConnectionId;


            var boardStateResposneDTO = new BoardStateResponseDTO()
            {
                GameId = boardStateConnectionRequestDTO.Data.GameId,
                CutableFigure = null!,
                IsReadyToEvent = boardStateConnectionRequestDTO.Data.IsReadyToEvent,
                From = boardStateConnectionRequestDTO.Data.From,
                To = boardStateConnectionRequestDTO.Data.To,
                OpponentConnectionId = selectedGameOpponentConnectionId,
                KingPosition = boardStateConnectionRequestDTO.Data.CheckedKingPosition,
                IsKingChecked = boardStateConnectionRequestDTO.Data.IsKingChecked,
                IsKingMate = boardStateConnectionRequestDTO.Data.IsKingMate,
                OpponentColor = boardStateConnectionRequestDTO.Data.OpponentColor,
                IsMyConnection = isMyConnection,
                Win = win
            };

            var sendBoardResposneDTO = ConnectionResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(boardStateResposneDTO, ChessGameResponseMessage.Draw);

            await _baseHubService.ReceiveBoardUpdateAsync(sendBoardResposneDTO);

            return ConnectionResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                   new BoardStateResponseDTO()
                   {
                       GameId = boardStateConnectionRequestDTO.Data.GameId,
                       Player = boardStateConnectionRequestDTO.Data.Player
                   },
                   ChessGameResponseMessage.MoveSuccessful,
                   System.Net.HttpStatusCode.OK);
        }
    }
}
