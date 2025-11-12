using BlazorServerSideClient.Contracts.Handlers;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

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
