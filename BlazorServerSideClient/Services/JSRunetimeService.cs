using Microsoft.JSInterop;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using static BlazorServerSideClient.Pages.GameHistory;

namespace BlazorServerSideClient.Services
{
    public class JSRunetimeService
    {
        private readonly IJSRuntime _js;
        private readonly ILogger<JSRunetimeService> _logger;

        public JSRunetimeService(IJSRuntime js, ILogger<JSRunetimeService> logger)
        {
            _logger = logger;
            _js = js;
        }

        public ValueTask<bool> InviteReceiverMessage(string inviterUserName)
                   => _js.SafeInvokeAsync<bool>(_logger, "confirm", $"{inviterUserName} invited you to a game!");

        public ValueTask InviteAcceptedMessage()
            => _js.SafeInvokeVoidAsync(_logger, "alert", "Your Invite was accepted!");

        public ValueTask DisableAllGameState(string gameClassName)
            => _js.SafeInvokeVoidAsync(_logger, "GameDiv.Disable", gameClassName);

        public ValueTask EnableAllGameState(string gameClassName)
            => _js.SafeInvokeVoidAsync(_logger, "GameDiv.Enable", gameClassName);

        public ValueTask ReceiveOptimalMoves(Position from,Position to)
            => _js.SafeInvokeVoidAsync(_logger, "ReceiveOptimalMoves.Show", from,to);
        public ValueTask WinNotifier_opponentLeft()
            => _js.SafeInvokeVoidAsync(_logger, "alert", "The opponent left. You win!");

        public ValueTask HideInviteModal()
            => _js.SafeInvokeVoidAsync(_logger, "inviteModal.hide");

        public ValueTask ShowInviteModal(int time, string userName)
            => _js.SafeInvokeVoidAsync(_logger, "inviteModal.show", time, userName);

        public ValueTask ShowPlayers(string player1_Name, string player2_Name)
            => _js.SafeInvokeVoidAsync(_logger, "Players.show", player1_Name, player2_Name);

        public ValueTask ShowBoardState<T>(string Blocks, int figureColor, DotNetObjectReference<T> dotNetRef) where T : class
            => _js.SafeInvokeVoidAsync(_logger, "BuildBoard.Build", Blocks, figureColor, dotNetRef);

        public ValueTask ShowMovableCutableBlocks(List<Block> cutablePositions, List<Block> movablePositions,List<CastlingInfosDTO> castlingInfosDTOs)
            => _js.SafeInvokeVoidAsync(_logger, "ShowMovableAndCutableBlocks.Paint", cutablePositions, movablePositions,castlingInfosDTOs);

        public ValueTask ClearSelectedBlocks(int figureColor)
            => _js.SafeInvokeVoidAsync(_logger, "ShowMovableAndCutableBlocks.Clear", figureColor);

        public ValueTask UpdateBoardAfterMove(Position from, Position to, int myColor)
            => _js.SafeInvokeVoidAsync(_logger, "UpdateBoardAfterMove.Move", from, to, myColor);

        public ValueTask UpdateBoardAfterCut(Position from, Position to, int myColor)
            => _js.SafeInvokeVoidAsync(_logger, "UpdateBoardAfterCut.Cut", from, to, myColor);

        public ValueTask KingCheckedNotifier(Position kingPosition)
            => _js.SafeInvokeVoidAsync(_logger, "KingCheckedNotification.Notify", kingPosition);

        public ValueTask KingMateNotifier(Position kingPosition, string currentPlayer, bool isWin)
            => _js.SafeInvokeVoidAsync(_logger, "KingMateNotification.Notify", kingPosition, currentPlayer, isWin);

        public ValueTask ReceiveBlockChangesHistory(List<Block> blockChangesHistory)
            => _js.SafeInvokeVoidAsync(_logger, "ReceiveBlockChangesHistory.Change", blockChangesHistory);

        public ValueTask NotifyOpponentUserDisconnected(string opponentUserName)
            => _js.SafeInvokeVoidAsync(_logger, "OpponentDisconnected.Notify", $"Your opponent {opponentUserName} has disconnected. You win!");
    }
    public static class JSRuntimeSafeExtensions
    {
        private const int RetryCount = 1;

        public static async ValueTask SafeInvokeVoidAsync(
            this IJSRuntime js,
            ILogger logger,
            string identifier,
            params object[] args)
        {
            for (var i = 0; i <= RetryCount; i++)
            {
                try
                {
                    await js.InvokeVoidAsync(identifier, args);
                    return;
                }
                catch (JSDisconnectedException ex)
                {
                    logger.LogWarning($"JS call '{identifier}' skipped (JSDisconnectedException).");
                    return;
                }
                catch (ObjectDisposedException ex)
                {
                    logger.LogWarning("JS call '{Identifier}' skipped (JSRuntime disposed).", identifier);
                    return;
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogWarning("JS call '{Identifier}' invalid (component disposed).", identifier);
                    return;
                }
                catch (Exception ex)
                {
                    if (i == RetryCount)
                        logger.LogError(ex, "Unexpected JS error while calling '{Identifier}'.", identifier);
                }
                await Task.Delay(200);
            }
        }

        public static async ValueTask<T?> SafeInvokeAsync<T>(
            this IJSRuntime js,
            ILogger logger,
            string identifier,
            params object[] args)
        {
            for (int i = 0; i <= RetryCount; i++)
            {
                try
                {

                    return await js.InvokeAsync<T>(identifier, args);
                }
                catch (JSDisconnectedException)
                {
                    logger.LogWarning($"JS call '{identifier}' skipped (JSDisconnectedException).");
                    return default;
                }
                catch (ObjectDisposedException)
                {
                    logger.LogWarning($"JS call '{identifier}' skipped (JSRuntime disposed).");

                    return default;
                }
                catch (InvalidOperationException)
                {
                    logger.LogWarning($"JS call '{identifier}' invalid (component disposed).");
                    return default;
                }
                catch (Exception ex)
                {
                    if (i == RetryCount)
                        logger.LogError(ex, "Unexpected JS error while calling '{Identifier}'.", identifier);
                }

                await Task.Delay(200);
            }

            return default;
        }
    }
}
