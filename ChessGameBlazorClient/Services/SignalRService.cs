using ChessGameBlazorClient.ServiceEndpoints;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
namespace ChessGameBlazorClient.UI.Services
{
    public class SignalRService
    {
        private readonly NavigationManager _navigationManager;
        private HubConnection _hubConnection;

        public SignalRService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public HubConnection GetHubConnection(IJSRuntime js)
        {
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(BasePaths.baseUrlHub, options =>
                    {
                        options.AccessTokenProvider = async () =>
                        {
                            // Read your auth cookie from browser
                            var token = await js.InvokeAsync<string>("getCookie", "access_token");
                            return token;
                        };
                        options.HttpMessageHandlerFactory = _ => new HttpClientHandler
                        {
                            UseCookies = true
                        };
                    })
                    .WithAutomaticReconnect()
                    .Build();
            }

            return _hubConnection;
        }

    }
}
