using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class GameRequestService : IGameRequestService
    {

        private readonly SignalRService _signalRService;
        public GameRequestService(SignalRService signalRService)
        {
            _signalRService = signalRService;
        }


        public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(Guid currentUserGuid)
        {
            var hubConnection = await _signalRService.GetHubConnection();
            return await hubConnection.
                InvokeAsync<
                    IResponseTypes<
                        UserConnectionResponseDTO, 
                        ChessGameResponseMessage>>
                        ("GetOnlinePlayersAsync", currentUserGuid);
        }
        public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(Guid gameId)
        {
            var hubConnection = await (_signalRService.GetHubConnection());
            return await hubConnection.
                InvokeAsync<
                    IResponseTypes<
                        UserConnectionResponseDTO, 
                        ChessGameResponseMessage>>
                        ("SendGameStateAsync", gameId);
        }

        public async Task ClearGameAsync(Guid gameId)
        {
            var hubConnection = await (_signalRService.GetHubConnection());
            await hubConnection.
                InvokeAsync<
                    IResponseTypes<
                        UserConnectionResponseDTO, 
                        ChessGameResponseMessage>>
                        ("ClearGameAsync", gameId);
        }

    }
}
