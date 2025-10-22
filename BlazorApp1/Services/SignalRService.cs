using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;
using System.Net.WebSockets;
using WebAssemblyChessGame.UI.ServiceEndpoints;

namespace WebAssemblyChessGame.UI.Services
{
    public class SignalRService
    {
        private readonly NavigationManager _navigationManager;
        private HubConnection _hubConnection;

        public SignalRService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public HubConnection GetHubConnection()
        {
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_navigationManager.ToAbsoluteUri($"{BasePaths.baseUrlHub}"), options =>
                    {
                        options.Transports = HttpTransportType.WebSockets;
                    })
                    .Build();
            }

            return _hubConnection;
        }
    }
}
