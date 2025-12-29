using BlazorServerSideClient.Contracts.Handlers;
using ChessGameBlazorClient.ServiceEndpoints;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
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
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IConnectionHandlerService _connectionHandlerService;
        private readonly IInvitationHandlerService _invitationHandlerService;
        private readonly IGameHandlerService _gameHandlerService;
        private readonly IHistoryWidgetHandlerService _historyWidgetHandlerService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ClaimsPrincipal _user;

        public SignalRService(
            IConnectionHandlerService connectionHandlerService,
            IInvitationHandlerService invitationHandlerService,
            IGameHandlerService gameHandlerService,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _gameHandlerService = gameHandlerService;
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

                var userName = _user.Claims?.First(claim => claim.Type == ClaimTypes.Name)?.Value;
                var userId = _user.Claims?.First(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

                var userIdAsGuid = Guid.Parse(userId!);

                await _hubConnection.SendAsync("AddConnectionAsync",
                  new AddUserConnectionRequestDTO()
                  {
                      userConnection = new UserConnectionDTO()
                      {
                          ConnectionId = _hubConnection.ConnectionId,
                          UserName = userName!
                      },
                      userGuid = userIdAsGuid
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

            await GetHubConnection();

            if (_hubConnection != null)
            {
                _hubConnection.On<KeyValuePair<Guid, UserConnectionDTO>>(
                    "ReceiveUpdatedUsers",
                    (userConnection) => _connectionHandlerService.ReceiveUpdatedUsers(userConnection)
                );

                _hubConnection.On<UserConnectionDTO, Guid, UserConnectionDTO, Guid>(
                    "ReceiveInvite",
                    (inviterUserConnection, inviterUserGuid, receiverUserConnection, receiverUserGuid) =>
                        _invitationHandlerService.ReceiveInvite(inviterUserConnection, inviterUserGuid,
                            receiverUserConnection, receiverUserGuid));

                _hubConnection.On<UserConnectionDTO, Guid, UserConnectionDTO, Guid, Guid>("InviteAcceptedAsync",
                    (
                            inviterUserConnection,
                            inviterUserGuid,
                            receiverUserConnection,
                            receiverUserGuid,
                            gameGuid) =>
                        _invitationHandlerService.InviteAcceptedAsync(
                            inviterUserConnection,
                            inviterUserGuid,
                            receiverUserConnection,
                            receiverUserGuid,
                            gameGuid));


                _hubConnection.On<
                    ResponseDTO<
                        ReceivePlayersResponseDTO,
                        ChessGameResponseMessage>>
                ("ReseivePlayersAsync", async (
                        connectionResponseDTO) => await _gameHandlerService.ReseivePlayersAsync(connectionResponseDTO)
                );

                _hubConnection.On<
                    ResponseDTO<
                        BoardStateResponseDTO,
                        ChessGameResponseMessage>>(
                    "ReceiveBoardUpdateAsync",
                    async (BoardStateResponseHandler) =>
                        await _gameHandlerService.ReceiveBoardUpdateAsync(BoardStateResponseHandler));


            }

            _hubConnection.Closed += async (error) =>
            {
                Console.WriteLine("Disconnected");
                await Task.CompletedTask;
            };
        }
    }
}
