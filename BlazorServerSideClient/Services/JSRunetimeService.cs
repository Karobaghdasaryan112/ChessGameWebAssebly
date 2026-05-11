using Microsoft.JSInterop;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace BlazorServerSideClient.Services
{
    public class JSRunetimeService(ILogger<JSRunetimeService> logger, IJSRuntime jsRunTime)
    {
        public IJSRuntime jsRunTime = jsRunTime;

        public async ValueTask ShowInviteModal(string userName)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "inviteModal.show", userName);
        }

        public async ValueTask<bool> InviteReceiverMessage(string inviterUserName)
        {
            return await jsRunTime.SafeInvokeAsync<bool>(logger, "confirm",
                $"{inviterUserName} invited you to a game!");
        }

        public async ValueTask InviteAcceptedMessage()
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "alert", "Your Invite was accepted!");
        }

        public async ValueTask DisableAllGameState(string gameClassName)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "GameDiv.Disable", gameClassName);
        }

        public async ValueTask EnableAllGameState(string gameClassName)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "GameDiv.Enable", gameClassName);
        }

        public async ValueTask ReceiveOptimalMoves(Position? from, Position? to)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "ReceiveOptimalMoves.Show", from, to);
        }

        public async ValueTask WinNotifier_opponentLeft()
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "alert", "The opponent left. You win!");
        }

        public async ValueTask HideInviteModal()
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "inviteModal.hide");
        }

        public async ValueTask NotesTrackerNotify(string eventType,
            string from, string to, bool isCapture)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "GameNotesTracker.notify", eventType, from, to, isCapture);
        }

        public async ValueTask ShowPlayers(string player1_Name, string player2_Name)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "Players.show", player1_Name, player2_Name);
        }

        public async ValueTask ShowBoardState<T>(string Blocks, int figureColor, DotNetObjectReference<T> dotNetRef)
            where T : class
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "BuildBoard.Build", Blocks, figureColor, dotNetRef);
        }

        public async ValueTask ShowErrorModal(string message)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "ErrorModal.Show", message);
        }

        public async ValueTask ShowMovableCutableBlocks(List<Block> cutablePositions, List<Block> movablePositions,
            List<CastlingInfosDTO> castlingInfosDTOs)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "ShowMovableAndCutableBlocks.Paint", cutablePositions,
                movablePositions, castlingInfosDTOs);
        }

        public async ValueTask ClearSelectedBlocks(int figureColor)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "ShowMovableAndCutableBlocks.Clear", figureColor);
        }

        public async ValueTask UpdateBoardAfterMove(Position from, Position to, int myColor)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "UpdateBoardAfterMove.Move", from, to, myColor);
        }

        public async ValueTask UpdateBoardAfterCut(Position from, Position to, int myColor)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "UpdateBoardAfterCut.Cut", from, to, myColor);
        }

        public async ValueTask KingCheckedNotifier(Position kingPosition)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "KingCheckedNotification.Notify", kingPosition);
        }

        public async ValueTask KingMateNotifier(Position kingPosition, string currentPlayer, bool isWin)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "KingMateNotification.Notify", kingPosition, currentPlayer,
                isWin);
        }

        public async ValueTask ReceiveBlockChangesHistory(List<Block> blockChangesHistory)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "ReceiveBlockChangesHistory.Change", blockChangesHistory);
        }

        public async ValueTask NotifyOpponentUserDisconnected(string opponentUserName)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "OpponentDisconnected.Notify",
                $"Your opponent {opponentUserName} has disconnected. You win!");
        }


        public async Task<TResponse> SendAsync<TRequest, TResponse>(string identifier, TRequest request)
        {
            return await jsRunTime.InvokeAsync<TResponse>(identifier, request);
        }

        public async Task InvokeAsync(string identifier, params object[] objects)
        {
            await jsRunTime.InvokeVoidAsync(identifier, objects);
        }

        public async Task NavigateTo(string path)
        {
            await jsRunTime.SafeInvokeVoidAsync(logger, "NavigateTo", path);
        }
    }

    public static class JSRuntimeSafeExtensions
    {
        public const int RetryCount = 5;

        public static async ValueTask SafeInvokeVoidAsync(
            this IJSRuntime js,
            ILogger logger,
            string identifier,
            params object[] args)
        {
            try
            {
                await js.InvokeVoidAsync(identifier, args);
                return;
            }
            catch (JSDisconnectedException)
            {
                logger.LogWarning($"JS call '{identifier}' skipped (JSDisconnectedException).");
                return;
            }
            catch (ObjectDisposedException)
            {
                logger.LogWarning("JS call '{Identifier}' skipped (JSRuntime disposed).", identifier);
                return;
            }
            catch (InvalidOperationException)
            {
                logger.LogWarning("JS call '{Identifier}' invalid (component disposed).", identifier);
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected JS error while calling '{Identifier}'.", identifier);
            }

            await Task.Delay(200);
        }

        public static async ValueTask<T?> SafeInvokeAsync<T>(
            this IJSRuntime js,
            ILogger logger,
            string identifier,
            params object[] args)
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
                logger.LogError(ex, "Unexpected JS error while calling '{Identifier}'.", identifier);
            }

            return await Task.FromResult(default(T));
        }
    }
}

