using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Services;
using ChessGameBlazorClient.ServiceEndpoints;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Security.Claims;

namespace ChessGameBlazorClient.UI.Services
{
    public class SignalRService
    {
        private HubConnection? _hubConnection;
        private string _hubConnectionId;
        private readonly IJSRuntime _jsRuneTime;
        private readonly ILogger<SignalRService> _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IConnectionHandlerService _connectionHandlerService;
        private readonly IInvitationHandlerService _invitationHandlerService;
        private readonly IGameHandlerService _gameHandlerService;
        private readonly IHistoryWidgetHandlerService _historyWidgetHandlerService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ClaimsPrincipal _user;
        private readonly JSRunetimeService _jSRunetimeService;
        private readonly DotNetObjectReference<SignalRService> _dotNetObjectReference;
        private IJSObjectReference? _jsModule;

        public SignalRService(
            ILogger<SignalRService> logger,
            IJSRuntime jSRuntime,
            JSRunetimeService jSRunetimeService,
            IConnectionHandlerService connectionHandlerService,
            IInvitationHandlerService invitationHandlerService,
            IGameHandlerService gameHandlerService,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _logger = logger;
            _dotNetObjectReference = DotNetObjectReference.Create(this);
            _jsRuneTime = jSRuntime;
            _jSRunetimeService = jSRunetimeService;
            _gameHandlerService = gameHandlerService;
            _invitationHandlerService = invitationHandlerService;
            _connectionHandlerService = connectionHandlerService;
            _authenticationStateProvider = authenticationStateProvider;
            _user = _authenticationStateProvider.GetAuthenticationStateAsync().GetAwaiter().GetResult().User;
        }

        public async Task<HubConnection> GetHubConnection()
        {
            //User Claims
            var userName = _user.Claims?.First(claim => claim.Type == ClaimTypes.Name)?.Value;
            var userId = _user.Claims?.First(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            var userGuid = Guid.Parse(userId!);

            //Initialize JS Module and Open SignalR Connection in JS if not already done
            await _semaphore.WaitAsync();
            try
            {
                if (_jsModule == null)
                {
                    //Connect to JS Module and Open SignalR Connection in JS
                    _jsModule = await _jsRuneTime.SafeInvokeAsync<IJSObjectReference>(
                                _logger,
                                "ChessGame",
                                BasePaths.baseUrlHub,
                                userName,
                                userGuid);

                    if (string.IsNullOrEmpty(_hubConnection?.ConnectionId) && _jsModule is not null)
                    {

                        //Wait until the connectionId is available in JS 
                        _hubConnectionId = await _jsModule.InvokeAsync<string>("getConnectionId");
                        return _hubConnection;
                    }

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


        //JsInvokable Methods
        [JSInvokable]
        public async Task ReceiveUpdatedUsers(KeyValuePair<Guid, UserConnectionDTO> userConnection)
        {
            _connectionHandlerService.ReceiveUpdatedUsers(new KeyValuePair<Guid, UserConnectionDTO>(userConnection.Key, userConnection.Value));
            await Task.CompletedTask;
        }

        [JSInvokable]
        public async Task ReceiveInvite(UserConnectionDTO inviterUserConnection, Guid inviterUserGuid, UserConnectionDTO receiverUserConnection, Guid receiverUserGuid)
        {
            _invitationHandlerService.ReceiveInvite(inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid);
            await Task.CompletedTask;
        }

        [JSInvokable]
        public async Task InviteAcceptedAsync(UserConnectionDTO inviterUserConnection, Guid inviterUserGuid, UserConnectionDTO receiverUserConnection, Guid receiverUserGuid, Guid gameGuid)
        {
            _invitationHandlerService.InviteAcceptedAsync(inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid, gameGuid);
            await Task.CompletedTask;
        }

        [JSInvokable]
        public async Task ReseivePlayersAsync(ResponseDTO<ReceivePlayersResponseDTO, ChessGameResponseMessage> connectionResponseDTO)
        {
            await _gameHandlerService.ReseivePlayersAsync(connectionResponseDTO);
        }



        public async Task DisconnectAsync()
        {
            await _hubConnection?.StopAsync();
        }

        public async Task RegisterConnectionHandlers()
        {

            await GetHubConnection();

            if (_hubConnection != null)
            {
                _hubConnection.On<
                    KeyValuePair<Guid, UserConnectionDTO>>(
                    "ReceiveUpdatedUsers",
                    (userConnection) => _connectionHandlerService.ReceiveUpdatedUsers(userConnection)
                );

                _hubConnection.On<
                    UserConnectionDTO,
                    Guid,
                    UserConnectionDTO,
                    Guid>(
                    "ReceiveInvite",
                    (inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid) =>
                        _invitationHandlerService.ReceiveInvite(inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid));

                _hubConnection.On<
                    UserConnectionDTO,
                    Guid,
                    UserConnectionDTO,
                    Guid,
                    Guid>("InviteAcceptedAsync",
                    (inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid, gameGuid) =>
                        _invitationHandlerService.InviteAcceptedAsync(inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid, gameGuid));


                _hubConnection.On<
                    ResponseDTO<
                        ReceivePlayersResponseDTO,
                        ChessGameResponseMessage>>
                ("ReseivePlayersAsync",
                async (connectionResponseDTO) => await _gameHandlerService.ReseivePlayersAsync(connectionResponseDTO));

                _hubConnection.On<
                    ResponseDTO<
                        BoardStateResponseDTO,
                        ChessGameResponseMessage>>(
                    "ReceiveBoardUpdateAsync",
                    async (BoardStateResponseHandler) =>
                        await _gameHandlerService.ReceiveBoardUpdateAsync(BoardStateResponseHandler));

                _hubConnection.On<KeyValuePair<Guid, UserConnectionDTO>>("DisconnectedNotification",
                    async (opponentUserConnection) =>
                {
                    await _gameHandlerService.NotifyOpponentUserDisconnected(opponentUserConnection);
                    _connectionHandlerService.DisconnectedNotification(opponentUserConnection);
                });

            }

            _hubConnection.Closed += async (error) =>
            {
                Console.WriteLine("Disconnected");
                await Task.CompletedTask;
            };
        }
    }
}
