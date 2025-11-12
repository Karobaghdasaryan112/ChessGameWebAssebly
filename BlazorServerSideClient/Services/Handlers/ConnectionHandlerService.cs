using BlazorServerSideClient.Contracts.Handlers;
using ChessGameBlazorClient.UI.Services;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using System.Collections.Concurrent;

namespace BlazorServerSideClient.Services.Handlers
{
    public class ConnectionHandlerService : IConnectionHandlerService
    {
        private JSRunetimeService _jsRunetimeService;
        private SignalRService _signalRService;
        public Action<KeyValuePair<Guid, UserConnectionResponseDTO>>? OnlinePlayersUpdated { get; set; }
        public ConnectionHandlerService(JSRunetimeService jSRunetimeService, SignalRService signalRService)
        {
            _signalRService = signalRService;
            _jsRunetimeService = jSRunetimeService;
        }
        public async Task ReceiveUpdatedUsers(KeyValuePair<Guid, UserConnectionResponseDTO> userConnection)
        {
            OnlinePlayersUpdated.Invoke(userConnection);
        }
    }
}
