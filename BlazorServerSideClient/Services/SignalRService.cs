using ChessGameBlazorClient.ServiceEndpoints;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using SharedResources.ChessGameResource.Models;
namespace ChessGameBlazorClient.UI.Services
{
    public class SignalRService
    {
        private HubConnection? _hubConnection;
        private readonly NavigationManager _navigation;
        private readonly IJSRuntime _js;

        public event Action<List<KeyValuePair<string, UserConnection>>>? OnlinePlayersUpdated;

        public SignalRService(NavigationManager navigation, IJSRuntime js)
        {
            _navigation = navigation;
            _js = js;
        }

        public HubConnection GetHubConnection()
        {
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(BasePaths.baseUrlHub)
                    .WithAutomaticReconnect()
                    .Build();

                RegisterHandlers();
            }
            return _hubConnection;
        }

        public async Task InitializeAsync(string userGuid, string userName)
        {
            await _hubConnection!.StartAsync();
            await _hubConnection.InvokeAsync("OnInitializedAsync", userGuid, new UserConnection
            {
                ConnectionId = _hubConnection.ConnectionId,
                UserName = userName
            });
        }

        private void RegisterHandlers()
        {
            _hubConnection!.On<List<KeyValuePair<string, UserConnection>>>("ReceiveOnlinePlayers", (players) =>
            {
                OnlinePlayersUpdated?.Invoke(players);
            });

            _hubConnection.On<UserConnection, string>("ReceiveInvite", async (inviter, targetGuid) =>
            {
                var accepted = await _js.InvokeAsync<bool>("confirm", $"{inviter.UserName} invited you to a game!");
                if (accepted)
                {
                    await _hubConnection.InvokeAsync("AcceptInvite", inviter, targetGuid);
                    _navigation.NavigateTo($"/game?player1={_hubConnection.ConnectionId}&player2={inviter.ConnectionId}");
                }
            });

            _hubConnection.On<UserConnection, string>("InviteAccepted", async (inviter, targetGuid) =>
            {
                await _js.InvokeVoidAsync("alert", "Your invite was accepted!");
                _navigation.NavigateTo($"/game?player1={_hubConnection.ConnectionId}&player2={targetGuid}");
            });
        }

        public async Task<List<KeyValuePair<string, UserConnection>>> GetOnlinePlayersAsync(string userGuid)
        {
            return await _hubConnection!.InvokeAsync<List<KeyValuePair<string, UserConnection>>>("GetOnlinePlayersAsync", userGuid);
        }

        public async Task SendInviteAsync(string playerId, string myPlayerId)
        {
            await _hubConnection!.InvokeAsync("SendInvite", playerId, myPlayerId);
        }
    }

}
