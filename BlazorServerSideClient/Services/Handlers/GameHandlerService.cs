using BlazorServerSideClient.Contracts.Handlers;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Handlers
{
    public class GameHandlerService(JSRunetimeService jSRuneTimeService) : IGameHandlerService
    {
        private DateTime _lastDisconnectNotificationAt = DateTime.MinValue;
        public static event Action<FigureColors, TimeSpan, TimeSpan>? OnTickReceived;
        public async Task ReseivePlayersAsync(
            ResponseDTO<ReceivePlayersResponseDTO, ChessGameResponseMessage> connectionResponseDto)
        {
            await jSRuneTimeService.ShowPlayers(connectionResponseDto.Data.Player1_UserConnectionDTO.UserName!,
                connectionResponseDto.Data.Player2_UserConnectionDTO?.UserName!);
        }
        

        public async Task ReceiveTick(FigureColors figureColor, TimeSpan whiteSpan, TimeSpan blackSpan)
        {
            Console.WriteLine("Service received tick from SignalR!"); 
            // Invoke the static event
            OnTickReceived?.Invoke(figureColor, whiteSpan, blackSpan);
        }

        public async Task ReceiveBoardUpdateAsync(
            ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage> gameStateconnectionResponseDto)
        {
            if (gameStateconnectionResponseDto.Data.IsReadyToEvent == IsReady.IsReadyToMove ||
                gameStateconnectionResponseDto.Data.IsReadyToEvent == IsReady.IsReadyToCastle)
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

        public async Task NotifyOpponentUserDisconnected(KeyValuePair<Guid, UserConnectionDTO> opponentUserConnection)
        {
            if (DateTime.UtcNow - _lastDisconnectNotificationAt < TimeSpan.FromSeconds(2))
            {
                return;
            }

            _lastDisconnectNotificationAt = DateTime.UtcNow;
            var opponentUserName = opponentUserConnection.Value.UserName;
            await jSRuneTimeService.NotifyOpponentUserDisconnected(opponentUserName!);
        }

        public async Task NotifyOpponentLeftWinAsync(string leavingPlayerName)
        {
            if (DateTime.UtcNow - _lastDisconnectNotificationAt < TimeSpan.FromSeconds(2))
                return;

            _lastDisconnectNotificationAt = DateTime.UtcNow;
            await jSRuneTimeService.NotifyOpponentUserDisconnected(leavingPlayerName);
        }

        public async Task RedirectToDashboardAsync()
            => await jSRuneTimeService.NavigateTo("/dashboard");
    }
}