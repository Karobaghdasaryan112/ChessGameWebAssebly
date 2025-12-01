using BlazorServerSideClient.Contracts.Handlers;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

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
        public async Task ReceiveBoardUpdateAsync(ConnectionResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage> gameStateconnectionResponseDTO)
        {
            if (gameStateconnectionResponseDTO.Data.IsKingChecked)
            {
                _jsService.KingCheckedNotifier(gameStateconnectionResponseDTO.Data.KingPosition);
            }

            if(gameStateconnectionResponseDTO.Data.IsReadyToEvent == IsReady.IsReadyToMove)
                await _jsService.UpdateBoardAfterMove(
                    gameStateconnectionResponseDTO.Data.From, 
                    gameStateconnectionResponseDTO.Data.To, 
                    (int)gameStateconnectionResponseDTO.Data.OpponentColor);
            else
                await _jsService.UpdateBoardAfterCut(
                    gameStateconnectionResponseDTO.Data.From,
                    gameStateconnectionResponseDTO.Data.To,
                    (int)gameStateconnectionResponseDTO.Data.OpponentColor);
        }
    }
}
