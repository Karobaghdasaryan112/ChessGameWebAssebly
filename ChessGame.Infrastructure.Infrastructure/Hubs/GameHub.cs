using Microsoft.AspNetCore.SignalR;
using SharedResources.ChessGameResource.Models;
using System.Collections.Concurrent;

namespace ChessGame.Infrastructure.Infrastructure.Hubs
{
    /// <summary>
    /// Represents the central SignalR hub that manages all real-time interactions in the chess game.
    /// This hub handles:
    /// <list type="bullet">
    /// <item><description>Connecting and disconnecting players.</description></item>
    /// <item><description>Sending, accepting, declining, and cancelling game invitations.</description></item>
    /// <item><description>Submitting and broadcasting chess moves between players.</description></item>
    /// </list>
    /// It ensures synchronized gameplay and player communication across connected clients.
    /// </summary>
    public class GameHub : Hub
    {
        private static readonly ConcurrentDictionary<string, UserConnection> _connections = new();

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = _connections.FirstOrDefault(x => x.Value.ConnectionId == Context.ConnectionId);
            if (!user.Equals(default))
            {
                _connections.TryRemove(user.Key, out _);
                await Clients.All.SendAsync("ReceiveOnlinePlayers", _connections.ToList());
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task OnInitializedAsync(string userGuid, UserConnection connection)
        {
            _connections[userGuid] = connection;
            await Clients.All.SendAsync("ReceiveOnlinePlayers", _connections.ToList());
        }

        public async Task<List<KeyValuePair<string, UserConnection>>> GetOnlinePlayersAsync(string userId)
        {
            var players = _connections
                .Where(c => c.Key != userId)
                .ToList();

            await Clients.Caller.SendAsync("ReceiveOnlinePlayers", players);
            return players;
        }

        public async Task SendInvite(string targetPlayerId, string myPlayerId)
        {
            if (_connections.TryGetValue(targetPlayerId, out var target))
            {
                await Clients.Client(target.ConnectionId)
                    .SendAsync("ReceiveInvite", _connections[myPlayerId], targetPlayerId);
            }
        }

        public async Task AcceptInvite(UserConnection inviter, string targetPlayerId)
        {
            await Clients.Client(inviter.ConnectionId)
                .SendAsync("InviteAccepted", inviter, targetPlayerId);
        }
    }

}
