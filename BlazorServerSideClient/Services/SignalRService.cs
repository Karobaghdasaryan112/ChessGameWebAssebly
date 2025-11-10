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

        public async Task<HubConnection> GetHubConnection()
        {
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(BasePaths.baseUrlHub)
                    .WithAutomaticReconnect()
                    .Build();

                RegisterHandlers();

                await _hubConnection.StartAsync();
            }
            while (string.IsNullOrEmpty(_hubConnection.ConnectionId))
            {
                await Task.Delay(500);
            }

            return _hubConnection;
        }

        public async Task InitializeAsync(string userGuid, string userName)
        {
            await GetHubConnection();
            await _hubConnection.InvokeAsync("OnInitializedAsync", userGuid, new UserConnection
            {
                ConnectionId = _hubConnection.ConnectionId ?? throw new ArgumentNullException(),
                UserName = userName
            });
        }

        public void RegisterHandlers()
        {
            _hubConnection!.On<List<KeyValuePair<string, UserConnection>>>("ReceiveOnlinePlayers", (players) =>
            {
                OnlinePlayersUpdated?.Invoke(players);
            });

            _hubConnection.On<KeyValuePair<string, UserConnection>, KeyValuePair<string, UserConnection>>("ReceiveInvite", async (inviter, target) =>
            {
                var accepted = await _js.InvokeAsync<bool>("confirm", $"{inviter.Value.UserName} invited you to a game!");

                if (accepted)
                {
                    var gameId = await _hubConnection.InvokeAsync<Guid>("AcceptInvite", inviter, target);
                    _navigation.NavigateTo($"/game?gameId={gameId}");
                }
            });

            _hubConnection.On<Guid>("InviteAccepted", async (gameId) =>
            {
                await _js.InvokeVoidAsync("alert", "Your invite was accepted!");
                _navigation.NavigateTo($"/game?GameId={gameId}");
            });

            _hubConnection.On<string>("WinNotifierAsync", async (userId) =>
            {
                 await _js.InvokeVoidAsync("alert", "the Opponent left the game.You Win!");
                _navigation.NavigateTo($"/Dashboard");
            });
        }

        public async Task<List<KeyValuePair<string, UserConnection>>> GetOnlinePlayersAsync(string userGuid)
        {
            await GetHubConnection();
            return await _hubConnection!.InvokeAsync<List<KeyValuePair<string, UserConnection>>>("GetOnlinePlayersAsync", userGuid);
        }

        public async Task SendInviteAsync(string playerId, string myPlayerId)
        {
            await _hubConnection!.InvokeAsync("SendInvite", playerId, myPlayerId);
        }
    }

}
