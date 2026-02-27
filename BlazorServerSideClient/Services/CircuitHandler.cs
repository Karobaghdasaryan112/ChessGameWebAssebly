using System.Security.Claims;
using BlazorServerSideClient.Helpers;
using BlazorServerSideClient.Services.Requests;
using ChessGame.Infrastructure.Infrastructure.Hubs;
using ChessGame.Infrastructure.Infrastructure.HubServices;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;

namespace BlazorServerSideClient.Services
{
    public class MyCircuitHandler : CircuitHandler
    {
        private readonly IHubContext<GameHub> _hubContext;

        public MyCircuitHandler(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public override async Task OnCircuitClosedAsync(
            Circuit circuit,
            CancellationToken cancellationToken)
        {
            var userConnectionGettingResult =
                CircuitHelper.TryGetValue(circuit, out var userConnection);

            if (!userConnectionGettingResult)
                return;

            await _hubContext.Clients.All.SendAsync(
                "DisconnectedNotification",
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