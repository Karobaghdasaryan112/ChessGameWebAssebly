using BlazorServerSideClient.Contracts.Handlers;
using Microsoft.JSInterop;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
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
        
        [JSInvokable]
        public async Task ReceiveBoardUpdateAsync(BoardStateResponseDTO  gameStateconnectionResponseDto)
        {

            if (gameStateconnectionResponseDto.IsReadyToEvent is IsReady.IsReadyToMove or IsReady.IsReadyToCastle)
            {
                if (gameStateconnectionResponseDto is { From: not null, To: not null })
                    await jSRuneTimeService.UpdateBoardAfterMove(
                        gameStateconnectionResponseDto.From,
                        gameStateconnectionResponseDto.To,
                        (int)gameStateconnectionResponseDto.OpponentColor);
            }
            else if (gameStateconnectionResponseDto is { From: not null, To: not null })
                await jSRuneTimeService.UpdateBoardAfterCut(
                    gameStateconnectionResponseDto.From,
                    gameStateconnectionResponseDto.To,
                    (int)gameStateconnectionResponseDto.OpponentColor);

            switch (gameStateconnectionResponseDto)
            {
                case { IsKingMate: true, KingPosition: not null }:
                    await jSRuneTimeService.KingMateNotifier(
                        gameStateconnectionResponseDto.KingPosition,
                        gameStateconnectionResponseDto.Player,
                        gameStateconnectionResponseDto.Win);
                    return;
                case { IsKingChecked: true, KingPosition: not null }:
                    await jSRuneTimeService.KingCheckedNotifier(gameStateconnectionResponseDto.KingPosition);
                    break;
            }
        }
        public async Task NotifyOpponentUserDisconnected(KeyValuePair<Guid, UserConnectionDTO> opponentUserConnection)
        {
            var opponentUserName = opponentUserConnection.Value.UserName;
            await jSRuneTimeService.NotifyOpponentUserDisconnected(opponentUserName!);
        }
    }
}
