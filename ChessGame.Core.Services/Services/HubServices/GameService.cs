using ChessGame.Core.Services.Contracts.Hub;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class GameService<T> : IGameService<T> where T : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IConnectionService<T> _connectionService;
        public GameService(IConnectionService<T> connectionService)
        {
            _connectionService = connectionService;
        }

        public Task ClearGameAsync(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<IResponseTypes<Dictionary<Guid, UserConnectionDTO>, ChessGameResponseMessage>> GetOnlinePlayersAsync(Guid currentUserGuid)
        {
            var onlinePlayers = _connectionService.
                CurrentConnectionState
                .Where(connectionKeyValuePair => connectionKeyValuePair.Key != currentUserGuid)
                .ToDictionary();
            return
                ChessGameResponse<Dictionary<Guid, UserConnectionDTO>>
                .CreateSuccessResponse(
                    onlinePlayers,
                    ChessGameResponseMessage.SuccessUserConnections,
                    System.Net.HttpStatusCode.OK);
        }


        public Task<IResponseTypes<UserConnectionDTO, ChessGameResponseMessage>> SendGameStateAsync(Guid gameId)
        {
            throw new NotImplementedException();
        }

    }
}
