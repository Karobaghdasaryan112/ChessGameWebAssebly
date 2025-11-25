using ChessGame.Core.Services.Contracts.Hub;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Collections.Concurrent;
using System.Net;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class ConnetionService<THub> : IConnectionService<THub> where THub : Microsoft.AspNetCore.SignalR.Hub
    {
        internal BaseHubService<THub> _baseHubService;
        internal static ConcurrentDictionary<Guid, UserConnectionDTO> _connections = new();

        public ConnetionService(BaseHubService<THub> baseHubService)
        {
            _baseHubService = baseHubService;
        }

        public ConcurrentDictionary<Guid, UserConnectionDTO> CurrentConnectionState => _connections;

        public ConnectionResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage> GetUserConnection(ConnectionRequestDTO<GetUserConnectionRequestDTO> getUserConnectionRequestDTO)
        {

            if (!_connections.TryGetValue(getUserConnectionRequestDTO.Data.UserGuid, out var currentUserConnection))
                return ConnectionResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>.
                    CreateErrorResponse(
                    new GetUserConnectionResponseDTO()
                    {
                        UserConnectionDTO = default
                    },
                    ChessGameResponseMessage.PlayerNotFound,
                    HttpStatusCode.NotFound,
                    new List<string> { $"user connection Not Found for UserId {getUserConnectionRequestDTO.Data.UserGuid}" });

            return ConnectionResponseDTO<GetUserConnectionResponseDTO, ChessGameResponseMessage>.
                  CreateSuccessResponse(
                  new GetUserConnectionResponseDTO()
                  {
                      UserConnectionDTO = currentUserConnection
                  },
                  ChessGameResponseMessage.UserConnectionFoundSuccess,
                  HttpStatusCode.Found);
        }

        public async Task<ConnectionResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(ConnectionRequestDTO<AddUserConnectionRequestDTO> addUserConnectionRequestDTO)
        {

            var existUserResult = GetUserConnection(
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
                    _connections[addUserConnectionRequestDTO.Data.userGuid].ConnectionId = addUserConnectionRequestDTO.Data.userConnection.ConnectionId;

                return
                    ConnectionResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>.
                    CreateSuccessResponse(
                        new AddUserConnectionResponseDTO()
                        {
                            IsAdded = true
                        },
                        ChessGameResponseMessage.ConnectionAddedSuccess,
                        HttpStatusCode.Created);
            }

            if (!_connections.TryAdd(addUserConnectionRequestDTO.Data.userGuid, addUserConnectionRequestDTO.Data.userConnection))
                return ConnectionResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>.
                  CreateErrorResponse(
                  new AddUserConnectionResponseDTO()
                  {
                      IsAdded = false
                  },
                  ChessGameResponseMessage.InternalServerError,
                  HttpStatusCode.InternalServerError,
                  new List<string> { $"cannot Added the UserConnection for User {addUserConnectionRequestDTO.Data.userGuid}" });

            await _baseHubService.SendUsersChange(new KeyValuePair<Guid, UserConnectionDTO>(addUserConnectionRequestDTO.Data.userGuid, addUserConnectionRequestDTO.Data.userConnection));

            return
                ConnectionResponseDTO<AddUserConnectionResponseDTO, ChessGameResponseMessage>.
                  CreateSuccessResponse(
                  new AddUserConnectionResponseDTO() { IsAdded = true },
                  ChessGameResponseMessage.ConnectionAddedSuccess,
                  HttpStatusCode.Created);

        }

        public async Task<ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsUserGuidAsync(ConnectionRequestDTO<RemoveUserConnectionRequestDTO> removeUserConnectionRequestDTO)
        {
            if (!_connections.TryRemove(removeUserConnectionRequestDTO.Data.UserGuid, out var removedConnection))
                return ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>.
                 CreateErrorResponse(
                 new RemoveUserConnectionResponseDTO()
                 {
                     IsRemoved = false
                 },
                 ChessGameResponseMessage.PlayerNotFound,
                 HttpStatusCode.NotFound,
                 new List<string> { $"cannot Delete the UserConnection for User {removeUserConnectionRequestDTO.Data.UserGuid}" });

            //TO DO _baseHubService.SwndRemoveUser

            return ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>.
                  CreateSuccessResponse(
                  new RemoveUserConnectionResponseDTO() { IsRemoved = true },
                  ChessGameResponseMessage.UserConnectionRemovedSuccess,
                  HttpStatusCode.Found);

        }
        public async Task<ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsConnectionIdAsync(RemoveUserConnectionRequestDTO removeUserConnectionRequestDTO)
        {
            var removeConnection = _connections.Where(connectionKvp => connectionKvp.Value.ConnectionId == removeUserConnectionRequestDTO.ConnectionId).FirstOrDefault();

            if (removeConnection.Equals(default))
                return ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>.
                CreateErrorResponse(
                new RemoveUserConnectionResponseDTO()
                {
                    IsRemoved = false
                },
                ChessGameResponseMessage.UserConnectionNotFound,
                HttpStatusCode.NotFound,
                new List<string> { $"cannot Delete the UserConnection ConnectionId-{removeUserConnectionRequestDTO.ConnectionId}" });

            if (!_connections.TryRemove(removeConnection))
                return ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>.
                 CreateErrorResponse(
                 new RemoveUserConnectionResponseDTO()
                 {
                     IsRemoved = false
                 },
                 ChessGameResponseMessage.PlayerNotFound,
                 HttpStatusCode.NotFound,
                 new List<string> { $"cannot Delete the UserConnection  ConnectionId-{removeUserConnectionRequestDTO.ConnectionId}" });

            //TO DO _baseHubService.SwndRemoveUser

            return ConnectionResponseDTO<RemoveUserConnectionResponseDTO, ChessGameResponseMessage>.
                  CreateSuccessResponse(
                  new RemoveUserConnectionResponseDTO()
                  {
                      IsRemoved = true
                  },
                  ChessGameResponseMessage.UserConnectionRemovedSuccess,
                  HttpStatusCode.Found);

        }

        public async Task<ConnectionResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>> SendBoardStateToOpponentClient(ConnectionRequestDTO<BoardStateRequestDTO> boardStateConnectionRequestDTO)
        {
            var selectedGameKeyValue = CurrentConnectionState.
           Where(gameId_UserConnection =>
               gameId_UserConnection.Value?.GameId ==
               boardStateConnectionRequestDTO.Data.GameId &&
               boardStateConnectionRequestDTO.Data.Player != gameId_UserConnection.Value?.UserName).
           Select(selectedGame_UserConnection => selectedGame_UserConnection.Value).ToList();

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
                CutableFigure = default,
                From = boardStateConnectionRequestDTO.Data.From,
                To = boardStateConnectionRequestDTO.Data.To,
                OpponentConnectionId = selectedGameOpponentConnectionId,
                OpponentColor = boardStateConnectionRequestDTO.Data.OpponentColor,
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
