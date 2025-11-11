using ChessGame.Core.Services.Contracts.Hub;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using System.Collections.Concurrent;
using System.Net;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class ConnetionService<THub> : IConnectionService<THub> where THub : Microsoft.AspNetCore.SignalR.Hub
    {
        private BaseHubService<THub> _baseHubService;
        internal static readonly ConcurrentDictionary<Guid, UserConnectionResponseDTO> _connections = new();

        public ConnetionService(BaseHubService<THub> baseHubService)
        {
            baseHubService = _baseHubService!;
        }

        public ConcurrentDictionary<Guid, UserConnectionResponseDTO> CurrentConnectionState => _connections;

        public IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage> GetUserConnection(Guid userGuid)
        {

            if (!_connections.TryGetValue(userGuid, out var currentUserConnection))
                return ChessGameResponse<UserConnectionResponseDTO>.
                    CreateErrorResponse(
                    ChessGameResponseMessage.PlayerNotFound,
                    HttpStatusCode.NotFound,
                    new List<string> { $"user connection Not Found for UserId {userGuid}" });

            return ChessGameResponse<UserConnectionResponseDTO>.
                  CreateSuccessResponse(
                  currentUserConnection,
                  ChessGameResponseMessage.UserConnectionFoundSuccess,
                  HttpStatusCode.Found);
        }

        public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> AddConnectionAsync(Guid userGuid, UserConnectionResponseDTO connection)
        {
            var existUserResult = GetUserConnection(userGuid);

            if (existUserResult.IsSuccess)
                return existUserResult;

            if (!_connections.TryAdd(userGuid, connection))
                return ChessGameResponse<UserConnectionResponseDTO>.
                  CreateErrorResponse(
                  ChessGameResponseMessage.InternalServerError,
                  HttpStatusCode.InternalServerError,
                  new List<string> { $"cannot Added the UserConnection for User {userGuid}" });

            await _baseHubService.SendUsersChange(_connections);

            return
                ChessGameResponse<UserConnectionResponseDTO>.
                  CreateSuccessResponse(
                  connection,
                  ChessGameResponseMessage.ConnectionAddedSuccess,
                  HttpStatusCode.Created);
        }

        public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsync(Guid userGuid)
        {
            if (!_connections.TryRemove(userGuid, out var removedConnection))
                return ChessGameResponse<UserConnectionResponseDTO>.
                 CreateErrorResponse(
                 ChessGameResponseMessage.PlayerNotFound,
                 HttpStatusCode.NotFound,
                 new List<string> { $"cannot Delete the UserConnection for User {userGuid}" });

            await _baseHubService.SendUsersChange(_connections);

            return ChessGameResponse<UserConnectionResponseDTO>.
                  CreateSuccessResponse(
                  removedConnection,
                  ChessGameResponseMessage.UserConnectionRemovedSuccess,
                  HttpStatusCode.Found);

        }
        public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> RemoveConnectionAsync(string connectionId)
        {
            var removeConnection = _connections.Where(connectionKvp => connectionKvp.Value.ConnectionId == connectionId).FirstOrDefault();
            if (removeConnection.Equals(default))
                return ChessGameResponse<UserConnectionResponseDTO>.
                CreateErrorResponse(
                ChessGameResponseMessage.UserConnectionNotFound,
                HttpStatusCode.NotFound,
                new List<string> { $"cannot Delete the UserConnection ConnectionId-{connectionId}" });

            if (!_connections.TryRemove(removeConnection))
                return ChessGameResponse<UserConnectionResponseDTO>.
                 CreateErrorResponse(
                 ChessGameResponseMessage.PlayerNotFound,
                 HttpStatusCode.NotFound,
                 new List<string> { $"cannot Delete the UserConnection  ConnectionId-{connectionId}" });

            await _baseHubService.SendUsersChange(_connections);

            return ChessGameResponse<UserConnectionResponseDTO>.
                  CreateSuccessResponse(
                  removeConnection.Value,
                  ChessGameResponseMessage.UserConnectionRemovedSuccess,
                  HttpStatusCode.Found);

        }
    }
}
