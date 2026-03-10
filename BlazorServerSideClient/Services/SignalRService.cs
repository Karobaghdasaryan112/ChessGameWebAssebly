using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Models;
using BlazorServerSideClient.Services;
using ChessGameBlazorClient.ServiceEndpoints;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace ChessGameBlazorClient.UI.Services
{
    public class SignalRService
    {
        private HubConnection? _hubConnection;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IConnectionHandlerService _connectionHandlerService;
        private readonly IInvitationHandlerService _invitationHandlerService;
        private readonly IGameHandlerService _gameHandlerService;
        private readonly IJSRuntime? _jsRuntime;
        private AuthenticationStateProvider _authenticationStateProvider;
        private DotNetObjectReference<IConnectionHandlerService> _dotNetObjectConnection;
        private DotNetObjectReference<IGameHandlerService> _dotNetObjectGame;
        private DotNetObjectReference<IInvitationHandlerService> _dotNetObjectInvitation;
        private SignalRConnectionInfoModel _signalRConnectionInfoModel;

        private ClaimsPrincipal _principal =>
            _authenticationStateProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult().User;

        private readonly ILogger<SignalRService> _logger;

        public SignalRService(
            ILogger<SignalRService> logger,
            IJSRuntime jsRuntime,
            JSRunetimeService jsRunetimeService,
            AuthenticationStateProvider authenticationStateProvider,
            IConnectionHandlerService connectionHandlerService,
            IInvitationHandlerService invitationHandlerService,
            IGameHandlerService gameHandlerService)
        {
            _logger = logger;
            _jsRuntime = jsRuntime;
            _authenticationStateProvider = authenticationStateProvider;
            _gameHandlerService = gameHandlerService;
            _invitationHandlerService = invitationHandlerService;
            _connectionHandlerService = connectionHandlerService;
        }

        public async Task<SignalRConnectionInfoModel> GetHubConnection(bool isCircuitHub = false)
        {
            if (!_principal.Identity.IsAuthenticated)
                return _signalRConnectionInfoModel;

            await _semaphore.WaitAsync();
            try
            {
                if (_signalRConnectionInfoModel == null)
                {
                    var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

                    var user = authState.User;
                    var userGuid = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var userName = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

                    await InitializeAsync();

                    var chessHub = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                        "ChessGame",
                        BasePaths.baseUrlHub,
                        userName,
                        userGuid,
                        _dotNetObjectInvitation,
                        _dotNetObjectGame,
                        _dotNetObjectConnection);

                    _signalRConnectionInfoModel =
                        await _jsRuntime.InvokeAsync<SignalRConnectionInfoModel>("getSignalRConnectionInfo");

                    return _signalRConnectionInfoModel;
                }

                return _signalRConnectionInfoModel;
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task DisconnectAsync()
        {
            await _hubConnection?.StopAsync()!;
        }


        public Task InitializeAsync()
        {
            _dotNetObjectConnection =
                DotNetObjectReference.Create(_connectionHandlerService);

            _dotNetObjectGame =
                DotNetObjectReference.Create(_gameHandlerService);

            _dotNetObjectInvitation =
                DotNetObjectReference.Create(_invitationHandlerService);

            return Task.CompletedTask;
        }


        //public async Task RegisterConnectionHandlers()
        //{

        //    await GetHubConnection();

        //    if (_hubConnection != null)
        //    {
        //        _hubConnection.On<
        //            KeyValuePair<Guid, UserConnectionDTO>>(
        //            "ReceiveUpdatedUsers",
        //            (userConnection) => _connectionHandlerService.ReceiveUpdatedUsers(userConnection.Key, userConnection.Value)
        //        );

        //        _hubConnection.On<
        //            UserConnectionDTO,
        //            Guid,
        //            UserConnectionDTO,
        //            Guid>(
        //            "ReceiveInvite",
        //            (inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid) =>
        //                _invitationHandlerService.ReceiveInvite(inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid));

        //        _hubConnection.On<
        //            UserConnectionDTO,
        //            Guid,
        //            UserConnectionDTO,
        //            Guid,
        //            Guid>("InviteAcceptedAsync",
        //            (inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid, gameGuid) =>
        //                _invitationHandlerService.InviteAcceptedAsync(inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid, gameGuid));


        //        _hubConnection.On<
        //            ResponseDTO<
        //                ReceivePlayersResponseDTO,
        //                ChessGameResponseMessage>>
        //        ("ReseivePlayersAsync",
        //        async (connectionResponseDTO) => await _gameHandlerService.ReseivePlayersAsync(connectionResponseDTO));

        //        _hubConnection.On<
        //            ResponseDTO<
        //                BoardStateResponseDTO,
        //                ChessGameResponseMessage>>(
        //            "ReceiveBoardUpdateAsync",
        //            async (BoardStateResponseHandler) =>
        //                await _gameHandlerService.ReceiveBoardUpdateAsync(BoardStateResponseHandler));

        //        _hubConnection.On<KeyValuePair<Guid, UserConnectionDTO>>("DisconnectedNotification",
        //            async (opponentUserConnection) =>
        //        {
        //            await _gameHandlerService.NotifyOpponentUserDisconnected(opponentUserConnection);
        //            _connectionHandlerService.DisconnectedNotification(opponentUserConnection);
        //        });

        //    }

        //    _hubConnection!.Closed += async (error) =>
        //    {
        //        Console.WriteLine("Disconnected");
        //        await Task.CompletedTask;
        //    };
        //}
    }
}