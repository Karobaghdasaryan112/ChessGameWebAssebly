using BlazorServerSideClient.Helpers;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace BlazorServerSideClient.Services
{
    public class MyCircuitHandler : CircuitHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public MyCircuitHandler(IServiceScopeFactory scopeFactory)
        {

            _serviceScopeFactory = scopeFactory;
        }

        public override async Task OnCircuitClosedAsync(
            Circuit circuit,
            CancellationToken cancellationToken)
        {
            var userConnectionResult =
                CircuitHelper.TryGetValue(circuit, out var userConnection);

            if (!userConnectionResult)
                return;

            var scope = _serviceScopeFactory.CreateScope();

            var signalRService = scope.ServiceProvider.GetRequiredService<SignalRService>();

            var hubConnection = await signalRService.GetHubConnection();

            await hubConnection.SendAsync(
                "SendDisconnectedUserNotificationAsync",
                new KeyValuePair<Guid, UserConnectionDTO>(
                    default,
                    new UserConnectionDTO
                    {
                        ConnectionId = userConnection.ConnectionId,
                        UserName = userConnection.UserName
                    }));


            await base.OnCircuitClosedAsync(circuit, cancellationToken);
        }
    }
}