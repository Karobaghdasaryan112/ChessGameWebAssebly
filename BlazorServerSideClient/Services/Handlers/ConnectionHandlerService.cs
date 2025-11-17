using BlazorServerSideClient.Contracts.Handlers;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace BlazorServerSideClient.Services.Handlers
{
    public class ConnectionHandlerService : IConnectionHandlerService
    {
        public Action<KeyValuePair<Guid, UserConnectionDTO>>? OnlinePlayersUpdated { get; set; }
        public void ReceiveUpdatedUsers(KeyValuePair<Guid, UserConnectionDTO> userConnection)
        {
            OnlinePlayersUpdated?.Invoke(userConnection);
        }
    }
}
