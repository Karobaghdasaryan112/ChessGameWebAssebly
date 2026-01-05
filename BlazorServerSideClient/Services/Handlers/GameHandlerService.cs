using BlazorServerSideClient.Contracts.Handlers;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Handlers
{
    public class GameHandlerService(JSRunetimeService jSRuneTimeService) : IGameHandlerService
    {
        public async Task ReseivePlayersAsync(
            ResponseDTO<ReceivePlayersResponseDTO, ChessGameResponseMessage> connectionResponseDto)
        {
            await jSRuneTimeService.ShowPlayers(connectionResponseDto.Data.Player1_UserConnectionDTO.UserName!,
                connectionResponseDto.Data.Player2_UserConnectionDTO?.UserName!);
        }

        public async Task ReceiveBoardUpdateAsync(
            ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage> gameStateconnectionResponseDto)
        {

            if (gameStateconnectionResponseDto.Data.IsReadyToEvent == IsReady.IsReadyToMove || gameStateconnectionResponseDto.Data.IsReadyToEvent == IsReady.IsReadyToCastle)
            {
                if (gameStateconnectionResponseDto.Data is { From: not null, To: not null })
                    await jSRuneTimeService.UpdateBoardAfterMove(
                        gameStateconnectionResponseDto.Data.From,
                        gameStateconnectionResponseDto.Data.To,
                        (int)gameStateconnectionResponseDto.Data.OpponentColor);
            }
            else if (gameStateconnectionResponseDto.Data is { From: not null, To: not null })
                await jSRuneTimeService.UpdateBoardAfterCut(
                    gameStateconnectionResponseDto.Data.From,
                    gameStateconnectionResponseDto.Data.To,
                    (int)gameStateconnectionResponseDto.Data.OpponentColor);

            switch (gameStateconnectionResponseDto.Data)
            {
                case { IsKingMate: true, KingPosition: not null }:
                    await jSRuneTimeService.KingMateNotifier(
                        gameStateconnectionResponseDto.Data.KingPosition,
                        gameStateconnectionResponseDto.Data.Player,
                        gameStateconnectionResponseDto.Data.Win);
                    return;
                case { IsKingChecked: true, KingPosition: not null }:
                    await jSRuneTimeService.KingCheckedNotifier(gameStateconnectionResponseDto.Data.KingPosition);
                    break;
            }
        }
    }
}
