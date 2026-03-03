using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Services;
using ChessGameBlazorClient.ServiceEndpoints;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.JSInterop;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.UserConnectionResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ChessGameBlazorClient.UI.Services
{
    public class SignalRService
    {
        private HubConnection? _hubConnection;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IConnectionHandlerService _connectionHandlerService;
        private readonly IInvitationHandlerService _invitationHandlerService;
        private readonly IGameHandlerService _gameHandlerService;
        private readonly JSRunetimeService _jsRunetimeService;
        private readonly IJSRuntime? _jsRuntime;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
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
            _jsRunetimeService = jsRunetimeService;
            _authenticationStateProvider = authenticationStateProvider;
            _gameHandlerService = gameHandlerService;
            _invitationHandlerService = invitationHandlerService;
            _connectionHandlerService = connectionHandlerService;
        }

        public async Task<HubConnection> GetHubConnection(bool isCircuitHub = false)
        {

            await _semaphore.WaitAsync();
            try
            {
                if (_hubConnection == null)
                {


                    var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

                    var user = authState.User;
                    var userGuid = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var userName = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                    
                    await _jsRuntime.SafeInvokeVoidAsync(_logger,"ChessGame",BasePaths.baseUrlHub,userName,userGuid);

                }

                return _hubConnection;
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

        public async Task RegisterConnectionHandlers()
        {

            await GetHubConnection();

            if (_hubConnection != null)
            {
                _hubConnection.On<
                    KeyValuePair<Guid, UserConnectionDTO>>(
                    "ReceiveUpdatedUsers",
                    (userConnection) => _connectionHandlerService.ReceiveUpdatedUsers(userConnection.Key, userConnection.Value)
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

            _hubConnection!.Closed += async (error) =>
            {
                Console.WriteLine("Disconnected");
                await Task.CompletedTask;
            };
        }
    }
}
