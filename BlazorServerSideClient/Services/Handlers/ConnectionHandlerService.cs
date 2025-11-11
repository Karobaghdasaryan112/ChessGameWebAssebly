using BlazorServerSideClient.Contracts.Handlers;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;

namespace BlazorServerSideClient.Services.Handlers
{
    public class ConnectionHandlerService : IConnectionHandlerService
    {
        private JSRunetimeService _jsRunetimeService;
        public Action<List<KeyValuePair<string, UserConnectionResponseDTO>>>? OnlinePlayersUpdated;
        public ConnectionHandlerService(JSRunetimeService jSRunetimeService)
        {
            _jsRunetimeService = jSRunetimeService;
        }
        public Task ReceiveOnlinePlayersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
