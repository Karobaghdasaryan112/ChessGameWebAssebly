using BlazorServerSideClient.Contracts.Handlers;
using Microsoft.JSInterop;
using SharedResources.ChessGameResource.Enums.Users;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace BlazorServerSideClient.Services.Handlers
{
    public class ConnectionHandlerService : IConnectionHandlerService
    {
        public Action<OnlinePlayerChangeType, KeyValuePair<Guid, UserConnectionDTO>>? OnlinePlayersUpdated { get; set; }

        [JSInvokable]
        public Task ReceiveUpdatedUsers(Guid userGuid, UserConnectionDTO connection)
        {
            OnlinePlayersUpdated?.Invoke(OnlinePlayerChangeType.Added,
                new KeyValuePair<Guid, UserConnectionDTO>(userGuid, connection));
            return Task.CompletedTask;
        }

        [JSInvokable]
        public void DisconnectedNotification(KeyValuePair<Guid, UserConnectionDTO> opponentUserConnection)
        {
            OnlinePlayersUpdated?.Invoke(OnlinePlayerChangeType.Removed, opponentUserConnection);
        }

        [JSInvokable]
        public void RemovedUserChangeNotification(Guid userGuid, UserConnectionDTO connectionn)
        {
            OnlinePlayersUpdated?.Invoke(OnlinePlayerChangeType.Removed,
                new KeyValuePair<Guid, UserConnectionDTO>(userGuid, connectionn));
        }
    }
}