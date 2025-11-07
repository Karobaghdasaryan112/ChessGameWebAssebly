using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using ServerSideClientUI.ServiceEndpoints;
using System;
using HubConnection = Microsoft.AspNetCore.SignalR.Client.HubConnection;

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

        public HubConnection GetHubConnection(string? jwtToken)
        {
            if (_hubConnection == null)
            {

              _hubConnection = new HubConnectionBuilder().Build();
            }

            return _hubConnection;
        }
    }
}
