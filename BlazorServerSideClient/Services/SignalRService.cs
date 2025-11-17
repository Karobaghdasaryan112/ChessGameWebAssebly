using BlazorServerSideClient.Contracts.Handlers;
using ChessGameBlazorClient.ServiceEndpoints;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using System.Security.Claims;
namespace ChessGameBlazorClient.UI.Services
{
    public class SignalRService
    {
        private HubConnection? _hubConnection;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IConnectionHandlerService _connectionHandlerService;
        private readonly IInvitationHandlerService _invitationHandlerService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ClaimsPrincipal _user;
        public SignalRService(IConnectionHandlerService connectionHandlerService, IInvitationHandlerService invitationHandlerService, AuthenticationStateProvider authenticationStateProvider)
        {
            _invitationHandlerService = invitationHandlerService;
            _connectionHandlerService = connectionHandlerService;
            _authenticationStateProvider = authenticationStateProvider;
            _user = _authenticationStateProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult().User;
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

                var userName = _user.Claims.First(claim => claim.Type == ClaimTypes.Name)?.Value;
                var userId = _user.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

                var userIdAsGuid = Guid.Parse(userId!);

                await _hubConnection.SendAsync("AddConnectionAsync",
                    new ConnectionRequestDTO<AddUserConnectionRequestDTO>()
                    {
                        Data = new AddUserConnectionRequestDTO()
                        {
                            userConnection = new UserConnectionDTO()
                            {
                                ConnectionId = _hubConnection.ConnectionId,
                                UserName = userName!
                            },
                            userGuid = userIdAsGuid
                        }
                    });

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

            _hubConnection.On<UserConnectionDTO, UserConnectionDTO>(
                "ReceiveInvite",
                (inviterUserConnection, receiverUserConnection) => _invitationHandlerService.ReceiveInvite(inviterUserConnection, receiverUserConnection));
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
