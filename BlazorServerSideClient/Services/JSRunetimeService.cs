using Microsoft.JSInterop;

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
    }
}
