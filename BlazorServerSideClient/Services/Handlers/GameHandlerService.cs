using BlazorServerSideClient.Contracts.Handlers;

namespace BlazorServerSideClient.Services.Handlers
{
    public class GameHandlerService : IGameHandlerService
    {
        private JSRunetimeService _jsRunetimeService;
        public GameHandlerService(JSRunetimeService jSRunetimeService)
        {
            _jsRunetimeService = jSRunetimeService; 
        }
    }
}
