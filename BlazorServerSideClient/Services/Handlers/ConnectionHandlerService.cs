using BlazorServerSideClient.Contracts.Handlers;
using Microsoft.JSInterop;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace BlazorServerSideClient.Services.Handlers
{
    public class ConnectionHandlerService : IConnectionHandlerService
    {
        public Action<KeyValuePair<Guid, UserConnectionDTO>>? OnlinePlayersUpdated { get; set; }

        [JSInvokable]
        public Task ReceiveUpdatedUsers(Guid userGuid, UserConnectionDTO connection)
        {
            OnlinePlayersUpdated?.Invoke(new KeyValuePair<Guid, UserConnectionDTO>(userGuid, connection));
            return Task.CompletedTask;
        }

        [JSInvokable]
        public void DisconnectedNotification(KeyValuePair<Guid, UserConnectionDTO> opponentUserConnection)
        {
            OnlinePlayersUpdated?.Invoke(opponentUserConnection);
        }
    }
}