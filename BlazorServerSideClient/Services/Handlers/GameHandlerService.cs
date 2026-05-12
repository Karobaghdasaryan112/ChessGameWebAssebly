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
            ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage> response)
        {
            var data = response.Data;
            if (data == null) return;

            // 1. Handle Game-Ending States First
            // If the game is over, we notify the user and stop further board animations.
            if (data is { IsKingMate: true, KingPosition: not null })
            {
                await jSRuneTimeService.KingMateNotifier(data.KingPosition, data.Player, data.Win);
                return; // Exit early: no need to process checks or moves after a mate
            }

            // 2. Handle Board Movement (Move, Castle, or Cut)
            if (data is { From: not null, To: not null })
            {
                bool isMoveOrCastle = data.IsReadyToEvent == IsReady.IsReadyToMove || 
                                      data.IsReadyToEvent == IsReady.IsReadyToCastle;

                if (isMoveOrCastle)
                {
                    await jSRuneTimeService.UpdateBoardAfterMove(data.From, data.To, (int)data.OpponentColor);
                }
                else // It's a Cut
                {
                    await jSRuneTimeService.UpdateBoardAfterCut(data.From, data.To, (int)data.OpponentColor);
                }
            }

            // 3. Handle Non-Fatal Alerts (Check)
            if (data is { IsKingChecked: true, KingPosition: not null })
            {
                await jSRuneTimeService.KingCheckedNotifier(data.KingPosition);
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