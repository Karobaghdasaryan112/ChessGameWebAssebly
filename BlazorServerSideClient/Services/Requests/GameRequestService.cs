using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class GameRequestService : IGameRequestService
    {

        private readonly SignalRService _signalRService;
        private readonly IConnectionHandlerService _connectionHandlerService;
        public GameRequestService(SignalRService signalRService, IConnectionHandlerService connectionHandlerService)
        {
            _connectionHandlerService = connectionHandlerService;
            _signalRService = signalRService;
        }


        public async Task<ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>>
            GetOnlinePlayersAsync(ConnectionRequestDTO<GetONlinePlayersRequestDTO> getOnlinePlayersRequestDTO)
        {
            var hubConnection = await _signalRService.GetHubConnection();
            var allGamersResult = await hubConnection.
                InvokeAsync<
                    ConnectionResponseDTO<
                        GetOnlinePlayersResponseDTO,
                        ChessGameResponseMessage>>
                        ("GetOnlinePlayersAsync", getOnlinePlayersRequestDTO);

            if (allGamersResult.IsSuccess)
                foreach (var guidAndConnections in allGamersResult.Data.OnlinePlayers!)
                    _connectionHandlerService.OnlinePlayersUpdated!.Invoke(guidAndConnections);

            return allGamersResult;
        }

        public async Task<ConnectionResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>>
            SendGameStateAsync(ConnectionRequestDTO<SendGameStateReqeustDTO> gameStateReqeustDTO)// Guid gameId
        {
            var hubConnection = await (_signalRService.GetHubConnection());
            return await hubConnection.
                InvokeAsync<
                    ConnectionResponseDTO<
                        SendGameStateResponseDTO,
                        ChessGameResponseMessage>>
                        ("SendGameStateAsync", gameStateReqeustDTO);
        }

        public async Task ClearGameAsync(Guid gameId)
        {
            var hubConnection = await (_signalRService.GetHubConnection());
            await hubConnection.
                InvokeAsync<
                    IResponseTypes<
                        UserConnectionDTO,
                        ChessGameResponseMessage>>
                        ("ClearGameAsync", gameId);
        }

    }
}
