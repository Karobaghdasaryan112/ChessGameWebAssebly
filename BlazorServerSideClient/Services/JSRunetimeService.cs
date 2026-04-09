using Microsoft.JSInterop;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace BlazorServerSideClient.Services
{
    public class JSRunetimeService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<JSRunetimeService> _logger;
        private const int DelayMs = 200;
        public readonly IJSRuntime jsRunTime;

        public JSRunetimeService(IServiceScopeFactory serviceScopeFactory, ILogger<JSRunetimeService> logger, IJSRuntime jsRunTime)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            this.jsRunTime = jsRunTime;
        }

        private async ValueTask SafeDelay() => await Task.Delay(DelayMs);


        public async ValueTask ShowInviteModal(string userName)
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "inviteModal.show", userName);
        }

        public async ValueTask<bool> InviteReceiverMessage(string inviterUserName)
        {
            await SafeDelay();
            return await jsRunTime.SafeInvokeAsync<bool>(_logger, "confirm", $"{inviterUserName} invited you to a game!");
        }

        public async ValueTask InviteAcceptedMessage()
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "alert", "Your Invite was accepted!");
        }

        public async ValueTask DisableAllGameState(string gameClassName)
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "GameDiv.Disable", gameClassName);
        }

        public async ValueTask EnableAllGameState(string gameClassName)
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "GameDiv.Enable", gameClassName);
        }

        public async ValueTask ReceiveOptimalMoves(Position? from, Position? to)
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "ReceiveOptimalMoves.Show", from, to);
        }

        public async ValueTask WinNotifier_opponentLeft()
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "alert", "The opponent left. You win!");
        }

        public async ValueTask HideInviteModal()
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "inviteModal.hide");
        }

        public async ValueTask ShowPlayers(string player1_Name, string player2_Name)
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "Players.show", player1_Name, player2_Name);
        }

        public async ValueTask ShowBoardState<T>(string Blocks, int figureColor, DotNetObjectReference<T> dotNetRef)
            where T : class
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "BuildBoard.Build", Blocks, figureColor, dotNetRef);
        }

        public async ValueTask ShowMovableCutableBlocks(List<Block> cutablePositions, List<Block> movablePositions,
            List<CastlingInfosDTO> castlingInfosDTOs)
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "ShowMovableAndCutableBlocks.Paint", cutablePositions,
                movablePositions, castlingInfosDTOs);
        }

        public async ValueTask ClearSelectedBlocks(int figureColor)
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "ShowMovableAndCutableBlocks.Clear", figureColor);
        }

        public async ValueTask UpdateBoardAfterMove(Position from, Position to, int myColor)
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "UpdateBoardAfterMove.Move", from, to, myColor);
        }

        public async ValueTask UpdateBoardAfterCut(Position from, Position to, int myColor)
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "UpdateBoardAfterCut.Cut", from, to, myColor);
        }

        public async ValueTask KingCheckedNotifier(Position kingPosition)
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "KingCheckedNotification.Notify", kingPosition);
        }

        public async ValueTask KingMateNotifier(Position kingPosition, string currentPlayer, bool isWin)
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "KingMateNotification.Notify", kingPosition, currentPlayer, isWin);
        }

        public async ValueTask ReceiveBlockChangesHistory(List<Block> blockChangesHistory)
        {
            await SafeDelay();
            await jsRunTime.SafeInvokeVoidAsync(_logger, "ReceiveBlockChangesHistory.Change", blockChangesHistory);
        }

        public async ValueTask NotifyOpponentUserDisconnected(string opponentUserName)
        {
            await SafeDelay();

            await jsRunTime.SafeInvokeVoidAsync(_logger, "OpponentDisconnected.Notify",
                $"Your opponent {opponentUserName} has disconnected. You win!");
        }


        public async Task<TResponse> SendAsync<TRequest, TResponse>(string identifier, TRequest request)
        {
            await SafeDelay();

            return await jsRunTime.InvokeAsync<TResponse>(identifier, request);
        }

        public async Task InvokeAsync(string identifier, params object[] objects)
        {
            await jsRunTime.InvokeVoidAsync(identifier, objects);
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
            for (var i = 0; i <= RetryCount; i++)
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