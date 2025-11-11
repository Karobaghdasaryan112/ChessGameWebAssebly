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
        public ValueTask<bool> InviteReceiverMessage(string inviterUserName)
        {
           return _js.InvokeAsync<bool>("confirm", $"{inviterUserName} invited you to a game!");
        }

    }
}
