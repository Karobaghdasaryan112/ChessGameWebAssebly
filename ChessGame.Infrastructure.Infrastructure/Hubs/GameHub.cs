using Microsoft.AspNetCore.SignalR;
using SharedResources.ChessGameResource.Models;
using System.Collections;
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
        //PlayerIds when they play in real Time

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
            if (!_connections.TryGetValue(userGuid, out var currentConnection))
                _connections[userGuid] = connection;
            await Clients.All.SendAsync("ReceiveOnlinePlayers", _connections.ToList());
        }

        public async Task<List<KeyValuePair<string, UserConnection>>> GetOnlinePlayersAsync(string userId)
        {
            var players = _connections
                .Where(c => c.Key != userId)
                .ToList();

            await Clients.All.SendAsync("ReceiveOnlinePlayers", players);
            return players;
        }
        public async Task GetPlayersInformation(Guid gameId)
        {
            var selectedGame = _connections.Where(kvp => kvp.Value.Gameinfo?.GameId == gameId).ToList();
            var returnValues = selectedGame.Select(selectedKvp => selectedKvp.Value).ToList();
            await Clients.Group(gameId.ToString()).SendAsync("ReceivePlayers", returnValues);
        }
        public async Task RemovePlayerFromGameAsync(string userId)
        {
            if(_connections.TryGetValue(userId,out var connection))
            {
                var anotherUserId = 
                    connection.Gameinfo.Players.Key == userId ? 
                    connection.Gameinfo.Players.Value : 
                    connection.Gameinfo.Players.Key;

                if (_connections.TryGetValue(anotherUserId, out var anotherConnection))
                {
                    await Clients.Client(anotherConnection.ConnectionId).SendAsync("WinNotifierAsync", anotherUserId);
                    anotherConnection.Gameinfo = null;
                }
                connection.Gameinfo = null;
            }
        }
        public async Task<UserConnection> IsUserInGame(string UserId)
        {
            _connections.TryGetValue(UserId, out var currentConnection);
            return currentConnection!;
        }

        public async Task SendInvite(string targetPlayerId, string myPlayerId)
        {
            if (_connections.TryGetValue(targetPlayerId, out var target))
            {
                await Clients.Client(target.ConnectionId)
                    .SendAsync("ReceiveInvite", _connections.Where(kvp => kvp.Key == myPlayerId).First(), _connections.Where(kvp => kvp.Key == targetPlayerId).First());
            }
        }

        public async Task<Guid> AcceptInvite(KeyValuePair<string, UserConnection> inviter, KeyValuePair<string, UserConnection> target)
        {
            var players = new KeyValuePair<string, string>(inviter.Key, target.Key);
            var gameGuid = Guid.NewGuid();
            if (_connections.TryGetValue(inviter.Key, out var inviterConnection))
                inviterConnection.Gameinfo = new Gameinfo() { GameId = gameGuid, Players = players };

            if (_connections.TryGetValue(target.Key, out var targetConnection))
                targetConnection.Gameinfo = new Gameinfo() { GameId = gameGuid, Players = players };

            await Groups.AddToGroupAsync(inviter.Value.ConnectionId, gameGuid.ToString());
            await Groups.AddToGroupAsync(target.Value.ConnectionId, gameGuid.ToString());

            await Clients.Client(inviter.Value.ConnectionId)
               .SendAsync("InviteAccepted", inviterConnection?.Gameinfo?.GameId);

            return gameGuid;
        }
    }
    public class IsInGameEqualityComparer : IEqualityComparer
    {
        public new bool Equals(object? x, object? y)
        {
            if (x == null || y == null) return false;

            return x.Equals(y);
        }

        public int GetHashCode(object obj)
        {
            if (obj == null) throw new ArgumentNullException();

            if (obj is KeyValuePair<string, string> pair)
            {
                return HashCode.Combine(pair.Value, pair.Key);
            }
            throw new ArgumentException();

        }

    }
}
