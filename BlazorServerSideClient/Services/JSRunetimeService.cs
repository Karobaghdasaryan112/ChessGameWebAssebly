using Microsoft.JSInterop;

namespace BlazorServerSideClient.Services
{
    public class JSRunetimeService
    {
        private readonly IJSRuntime _js;
        public JSRunetimeService(IJSRuntime jS)
        {
            _js = jS;
        }
        public ValueTask<bool> InviteReceiverMessage(string inviterUserName) =>  _js.InvokeAsync<bool>("confirm", $"{inviterUserName} invited you to a game!");
        public async ValueTask InviteAcceptedMessage() => await _js.InvokeVoidAsync("alert", "Your Invie was accepted!");

        public async ValueTask WinNotifier_opponentLeft() => await _js.InvokeVoidAsync("alert", "the Opponent left the game.You Win!");

    }
}
