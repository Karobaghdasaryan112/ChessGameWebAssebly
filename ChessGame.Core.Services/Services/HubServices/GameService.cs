using ChessGame.Core.Services.Contracts.Hub;
using Microsoft.AspNetCore.SignalR;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
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

        public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(Guid currentUserGuid)
        {
            var onlinePlayers = _connectionService.
                CurrentConnectionState
                .Where(connectionKeyValuePair => connectionKeyValuePair.Key != currentUserGuid)
                .Select(selectedKvp => selectedKvp.Value)
                .ToList();
            return
                ChessGameResponse<UserConnectionResponseDTO>
                .CreateSuccessResponse(
                    onlinePlayers,
                    ChessGameResponseMessage.SuccessUserConnections,
                    System.Net.HttpStatusCode.OK);
        }


        public Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(Guid gameId)
        {
            throw new NotImplementedException();
        }

    }
}
