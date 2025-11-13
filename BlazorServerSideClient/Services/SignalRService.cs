using BlazorServerSideClient.Contracts.Handlers;
using ChessGameBlazorClient.ServiceEndpoints;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
namespace ChessGameBlazorClient.UI.Services
{
    public class SignalRService
    {
        private HubConnection? _hubConnection;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IConnectionHandlerService _connectionHandlerService;
        public SignalRService(IConnectionHandlerService connectionHandlerService)
        {
            _connectionHandlerService = connectionHandlerService;
        }
        public async Task<HubConnection> GetHubConnection()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_hubConnection == null)
                {
                    _hubConnection = new HubConnectionBuilder()
                        .WithUrl(BasePaths.baseUrlHub)
                        .WithAutomaticReconnect()
                        .Build();

                    await _hubConnection.StartAsync();
                }
                while (string.IsNullOrEmpty(_hubConnection.ConnectionId))
                {
                    await Task.Delay(200);
                }
                return _hubConnection;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task RegisterConnectionHandlers()
        {
            _hubConnection.On<KeyValuePair<Guid, UserConnectionDTO>>(
                    "ReceiveUpdatedUsers",
                    (userConnection) => _connectionHandlerService.ReceiveUpdatedUsers(userConnection)
                );
        }
        //public async Task InitializeAsync(string userGuid, string userName)
        //{
        //    await GetHubConnection();
        //    //await _hubConnection.InvokeAsync("OnInitializedAsync", userGuid, new UserConnection
        //    //{
        //    //    ConnectionId = _hubConnection.ConnectionId ?? throw new ArgumentNullException(),
        //    //    UserName = userName
        //    //});
        //}

        //public void RegisterHandlers()
        //{
        //    //Invitation Handlers Registration
        //    _hubConnection!.On<List<KeyValuePair<string, UserConnection>>>("ReceiveOnlinePlayers", (players) =>
        //    {
        //        OnlinePlayersUpdated?.Invoke(players);
        //    });

        //    _hubConnection.On<KeyValuePair<string, UserConnectionResponseDTO>, KeyValuePair<string, UserConnectionResponseDTO>>("ReceiveInvite", async (inviter, target) =>
        //    {
        //        var accepted = _jSRunetimeService.InviteReceiverMessage(inviter.Value.UserName);

        //        if (accepted)
        //        {
        //            var gameId = await _hubConnection.InvokeAsync<Guid>("AcceptInvite", inviter, target);
        //            _navigation.NavigateTo($"/game?gameId={gameId}");
        //        }
        //    });

        //    //Connection Handlers Registration

        //    _hubConnection.On<Guid>("InviteAccepted", async (gameId) =>
        //    {
        //        await _js.InvokeVoidAsync("alert", "Your invite was accepted!");
        //        _navigation.NavigateTo($"/game?GameId={gameId}");
        //    });

        //    _hubConnection.On<string>("WinNotifierAsync", async (userId) =>
        //    {
        //        await _js.InvokeVoidAsync("alert", "the Opponent left the game.You Win!");
        //        _navigation.NavigateTo("/Dashboard");
        //    });

        //    //Game Handlers Regisration

        //}

        //public async Task<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(Guid currentUserGuid)
        //{
        //    await GetHubConnection();
        //    return await _hubConnection!.InvokeAsync<IResponseTypes<UserConnectionResponseDTO, ChessGameResponseMessage>>("GetOnlinePlayersAsync", currentUserGuid);
        //}

        //public async Task SendInviteAsync(string playerId, string myPlayerId)
        //{
        //    await _hubConnection!.InvokeAsync("SendInvite", playerId, myPlayerId);
        //}
    }

}
