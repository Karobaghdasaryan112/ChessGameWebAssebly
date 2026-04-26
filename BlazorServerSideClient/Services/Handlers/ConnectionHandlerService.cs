using BlazorServerSideClient.Contracts.Handlers;
using Microsoft.JSInterop;
using SharedResources.ChessGameResource.Enums.Users;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace BlazorServerSideClient.Services.Handlers
{
    public class ConnectionHandlerService : IConnectionHandlerService
    {
        public Action<OnlinePlayerChangeType, KeyValuePair<Guid, UserConnectionDTO>>? OnlinePlayersUpdated { get; set; }
        
        public void ReceiveUpdatedUsers(KeyValuePair<Guid, UserConnectionDTO> userConnection,OnlinePlayerChangeType onlinePlayerChangeType)
        {
            OnlinePlayersUpdated?.Invoke(onlinePlayerChangeType,
                new KeyValuePair<Guid, UserConnectionDTO>(userConnection.Key, userConnection.Value));
        }
        
        public void DisconnectedNotification(KeyValuePair<Guid, UserConnectionDTO> opponentUserConnection)
        {
            OnlinePlayersUpdated?.Invoke(OnlinePlayerChangeType.Removed, opponentUserConnection);
        }
        
        public void RemovedUserChangeNotification(Guid userGuid, UserConnectionDTO connectionn)
        {
            OnlinePlayersUpdated?.Invoke(OnlinePlayerChangeType.Removed,
                new KeyValuePair<Guid, UserConnectionDTO>(userGuid, connectionn));
        }
    }
}