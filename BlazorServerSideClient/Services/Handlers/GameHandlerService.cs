using BlazorServerSideClient.Contracts.Handlers;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Text.Json;

namespace BlazorServerSideClient.Services.Handlers
{
    public class GameHandlerService : IGameHandlerService
    {
        private readonly JSRunetimeService _jsService;
        public GameHandlerService(JSRunetimeService jSRunetimeService)
        {
            _jsService = jSRunetimeService;
        }
        public async Task ReseivePlayersAsync(ConnectionResponseDTO<ReceivePlayersResponseDTO, ChessGameResponseMessage> connectionResponseDTO)
        {
           await _jsService.ShowPlayers(connectionResponseDTO.Data.Player1_UserConnectionDTO.UserName!, connectionResponseDTO.Data.Player2_UserConnectionDTO?.UserName!);
        }
        public async Task ReceiveBoardStateAsync(ConnectionResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage> gameStateconnectionResponseDTO)
        {
            await _jsService.ShowBoardState(JsonSerializer.Serialize(gameStateconnectionResponseDTO.Data.Board.GetBlocks));
        }
    }
}
