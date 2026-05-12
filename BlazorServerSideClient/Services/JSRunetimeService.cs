using Microsoft.JSInterop;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using Microsoft.Extensions.Logging;

namespace BlazorServerSideClient.Services
{
    public class JSRunetimeService(ILogger<JSRunetimeService> logger, IJSRuntime jsRunTime)
    {
        private readonly IJSRuntime _jsRunTime = jsRunTime;
        private readonly ILogger<JSRunetimeService> _logger = logger;

        // --- Invitation & Modals ---
        public async ValueTask ShowInviteModal(string userName) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "inviteModal.show", userName);

        public async ValueTask<bool> InviteReceiverMessage(string inviterUserName) =>
            await _jsRunTime.SafeInvokeAsyncWithRetry<bool>(_logger, "confirm", $"{inviterUserName} invited you to a game!");

        public async ValueTask InviteAcceptedMessage() =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "alert", "Your Invite was accepted!");

        public async ValueTask HideInviteModal() =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "inviteModal.hide");

        public async ValueTask ShowErrorModal(string message) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "ErrorModal.Show", message);

        // --- Board & Piece Visualization ---
        public async ValueTask ShowBoardState<T>(string Blocks, int figureColor, DotNetObjectReference<T> dotNetRef) where T : class =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "BuildBoard.Build", Blocks, figureColor, dotNetRef);

        public async ValueTask ShowMovableCutableBlocks(List<Block> cutablePositions, List<Block> movablePositions, List<CastlingInfosDTO> castlingInfosDTOs) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "ShowMovableAndCutableBlocks.Paint", cutablePositions, movablePositions, castlingInfosDTOs);

        public async ValueTask ClearSelectedBlocks(int figureColor) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "ShowMovableAndCutableBlocks.Clear", figureColor);

        // --- Movement & History Updates ---
        public async ValueTask UpdateBoardAfterMove(Position from, Position to, int myColor) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "UpdateBoardAfterMove.Move", from, to, myColor);

        public async ValueTask UpdateBoardAfterCut(Position from, Position to, int myColor) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "UpdateBoardAfterCut.Cut", from, to, myColor);

        public async ValueTask ReceiveBlockChangesHistory(List<Block> blockChangesHistory) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "ReceiveBlockChangesHistory.Change", blockChangesHistory);

        // --- Game Logic Notifiers ---
        public async ValueTask DisableAllGameState(string gameClassName) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "GameDiv.Disable", gameClassName);

        public async ValueTask EnableAllGameState(string gameClassName) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "GameDiv.Enable", gameClassName);

        public async ValueTask KingCheckedNotifier(Position kingPosition) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "KingCheckedNotification.Notify", kingPosition);

        public async ValueTask KingMateNotifier(Position kingPosition, string currentPlayer, bool isWin) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "KingMateNotification.Notify", kingPosition, currentPlayer, isWin);

        public async ValueTask NotifyOpponentUserDisconnected(string opponentUserName) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "OpponentDisconnected.Notify", $"Your opponent {opponentUserName} has disconnected. You win!");

        public async ValueTask WinNotifier_opponentLeft() =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "alert", "The opponent left. You win!");

        // --- Utility ---
        public async ValueTask NotesTrackerNotify(string eventType, string from, string to, bool isCapture) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "GameNotesTracker.notify", eventType, from, to, isCapture);

        public async ValueTask ShowPlayers(string player1_Name, string player2_Name) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "Players.show", player1_Name, player2_Name);

        public async ValueTask ReceiveOptimalMoves(Position? from, Position? to) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "ReceiveOptimalMoves.Show", from, to);

        public async Task<TResponse?> SendAsync<TRequest, TResponse>(string identifier, TRequest request) =>
            await _jsRunTime.SafeInvokeAsyncWithRetry<TResponse>(_logger, identifier, request);

        public async Task InvokeAsync(string identifier, params object[] objects) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, identifier, objects);

        public async Task NavigateTo(string path) =>
            await _jsRunTime.SafeInvokeWithRetryAsync(_logger, "NavigateTo", path);
    }

    public static class JSRuntimeSafeExtensions
    {
        private const int MaxRetries = 3;
        private const int DelayBetweenRetriesMs = 500; // Faster retry for better UX

        public static async ValueTask SafeInvokeWithRetryAsync(
            this IJSRuntime js,
            ILogger logger,
            string identifier,
            params object[] args)
        {
            for (int i = 0; i < MaxRetries; i++)
            {
                try
                {
                    await js.InvokeVoidAsync(identifier, args);
                    return;
                }
                catch (Exception ex) when (IsTransient(ex))
                {
                    logger.LogWarning("JS Call '{Id}' failed (Attempt {Attempt}). Circuit may be disconnected. Retrying...", identifier, i + 1);
                    if (i < MaxRetries - 1) await Task.Delay(DelayBetweenRetriesMs);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Non-recoverable JS error in '{Id}'", identifier);
                    return;
                }
            }
        }

        public static async ValueTask<T?> SafeInvokeAsyncWithRetry<T>(
            this IJSRuntime js,
            ILogger logger,
            string identifier,
            params object[] args)
        {
            for (int i = 0; i < MaxRetries; i++)
            {
                try
                {
                    return await js.InvokeAsync<T>(identifier, args);
                }
                catch (Exception ex) when (IsTransient(ex))
                {
                    logger.LogWarning("JS Call '{Id}' failed (Attempt {Attempt}). Retrying...", identifier, i + 1);
                    if (i < MaxRetries - 1) await Task.Delay(DelayBetweenRetriesMs);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Non-recoverable JS error in '{Id}'", identifier);
                    break;
                }
            }
            return default;
        }

        private static bool IsTransient(Exception ex) =>
            ex is JSDisconnectedException || 
            ex is TaskCanceledException || 
            ex is ObjectDisposedException ||
            (ex is InvalidOperationException && ex.Message.Contains("interop"));
    }
}