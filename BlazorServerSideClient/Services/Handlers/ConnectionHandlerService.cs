using BlazorServerSideClient.Contracts.Handlers;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace BlazorServerSideClient.Services.Handlers
{
    public class ConnectionHandlerService : IConnectionHandlerService
    {
        private JSRunetimeService _jsRunetimeService;
        public Action<KeyValuePair<Guid, UserConnectionDTO>>? OnlinePlayersUpdated { get; set; }
        public ConnectionHandlerService(JSRunetimeService jSRunetimeService)
        {
            _jsRunetimeService = jSRunetimeService;
        }
        public void ReceiveUpdatedUsers(KeyValuePair<Guid, UserConnectionDTO> userConnection)
        {
            try
            {
                OnlinePlayersUpdated?.Invoke(userConnection);
            }catch(Exception ex)
            {
                var msg = ex.Message;
            }
        }
    }
}
