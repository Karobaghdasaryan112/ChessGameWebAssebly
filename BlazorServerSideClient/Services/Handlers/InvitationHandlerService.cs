using BlazorServerSideClient.Contracts.Handlers;

namespace BlazorServerSideClient.Services.Handlers
{
    public class InvitationHandlerService : IInvitationHandlerService
    {
        private JSRunetimeService _jsRunetimeService;
        public InvitationHandlerService(JSRunetimeService jSRunetimeService)
        {
            _jsRunetimeService = jSRunetimeService;
        }
    }
}
