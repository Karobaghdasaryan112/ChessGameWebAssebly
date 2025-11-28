using BlazorServerSideClient.Pages;
using Microsoft.JSInterop;
using SharedResources.ChessGameResource.Models;

namespace BlazorServerSideClient.Services
{
    public class JSRunetimeService
    {
        private readonly IJSRuntime _js;

        public JSRunetimeService(IJSRuntime js)
        {
            _js = js;
        }

        public ValueTask<bool> InviteReceiverMessage(string inviterUserName)
            => _js.InvokeAsync<bool>("confirm", $"{inviterUserName} invited you to a game!");

        public ValueTask InviteAcceptedMessage()
            => _js.InvokeVoidAsync("alert", "Your Invite was accepted!");

        public ValueTask WinNotifier_opponentLeft()
            => _js.InvokeVoidAsync("alert", "The opponent left. You win!");

        public ValueTask HideInviteModal()
            => _js.InvokeVoidAsync("inviteModal.hide");

        public ValueTask ShowInviteModal(int time,string userName)
            => _js.InvokeVoidAsync("inviteModal.show",time,userName);

        public ValueTask ShowPlayers(string player1_Name, string player2_Name) 
            => _js.InvokeVoidAsync("Players.show", player1_Name, player2_Name);

        public ValueTask ShowBoardState(string Blocks,int figureColor,DotNetObjectReference<Game> dotNetRef) 
            => _js.InvokeVoidAsync("BuildBoard.Build", Blocks, figureColor, dotNetRef);

        public ValueTask ShowMovableCutableBlocks(List<Block> cutablePositions, List<Block> movablePositions)
            => _js.InvokeVoidAsync("ShowMovableAndCutableBlocks.Paint", cutablePositions, movablePositions);

        public ValueTask ClearSelectedBlocks(int figureColor) 
            => _js.InvokeVoidAsync("ShowMovableAndCutableBlocks.Clear", figureColor);

        public ValueTask UpdateBoardAfterMove(Position from,Position to,int myColor) 
            => _js.InvokeVoidAsync("UpdateBoardAfterMove.Move", from, to, myColor);

        public ValueTask UpdateBoardAfterCut(Position from,Position to,int myColor) 
            => _js.InvokeVoidAsync("UpdateBoardAfterCut.Cut", from, to, myColor);

        public ValueTask KingCheckedNotifier(Position kingPosition)
            => _js.InvokeVoidAsync("KingCheckedNotification.Notify", kingPosition);

    }
}
