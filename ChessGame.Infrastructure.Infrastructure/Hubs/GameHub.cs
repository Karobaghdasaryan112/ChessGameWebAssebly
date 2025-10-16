using ChessGame.Core.Services.MediatR.Requests.Commands;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Requests;
using SharedResources.Responses.ResponseMessages;
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
        private readonly ILogger<GameHub> _logger;
        private readonly IMediator _mediator;
        public GameHub(
            ILogger<GameHub> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Stores active connections between clients and their corresponding players.
        /// Key — PlayerId ConnectionId (identifier of the player associated with the connection),
        /// Value — ConnectionId (unique SignalR connection identifier).
        /// </summary>
        //(playerId -- connectionId)
        private static readonly ConcurrentDictionary<string, string> _connections = new();


        /// <summary>
        /// Stores pending (unaccepted) game invitations.
        /// Key — TargetPlayerId (identifier of the player who received the invite),
        /// Value — InviterConnectionId (connection identifier of the player who sent the invite).
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> _pendingInvites = new();


        /// <summary>
        /// Called when a new client successfully connects to the hub.
        /// Adds the player's connection to the active connections dictionary,
        /// using the player's unique identifier (UserIdentifier or ConnectionId).
        /// </summary>
        /// <returns>A completed task representing the asynchronous operation.</returns>
        public override Task OnConnectedAsync()
        {
            var playerId = Context.UserIdentifier ?? Context.ConnectionId;
            _connections[playerId] = Context.ConnectionId;
            return base.OnConnectedAsync();
        }


        /// <summary>
        /// Called when a client disconnects from the hub.
        /// Removes the player's connection from the active connections dictionary
        /// and clears any pending game invitations associated with that connection.
        /// </summary>
        /// <param name="exception">The exception that triggered the disconnect, if any.</param>
        /// <returns>A completed task representing the asynchronous operation.</returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryRemove(Context.UserIdentifier ?? Context.ConnectionId, out var connectionId))
            {
                var invitesToRemove = _pendingInvites.Where(kvp => kvp.Value == connectionId).Select(kvp => kvp.Key).ToList();

                foreach (var targetPlayerId in invitesToRemove)
                    _pendingInvites.TryRemove(targetPlayerId, out _);

            }
            return base.OnDisconnectedAsync(exception);
        }


        /// <summary>
        /// Sends a game invitation to the specified target player.
        /// If the target player is currently connected, an invite is sent to their client,
        /// and the invite is recorded in the pending invites dictionary.
        /// Otherwise, notifies the sender that the target player is unavailable.
        /// </summary>
        /// <param name="targetPlayerId">The unique identifier of the player to invite.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendInvite(string targetPlayerId)
        {
            if (_connections.TryGetValue(targetPlayerId, out var connectionId))
            {
                _pendingInvites[targetPlayerId] = Context.ConnectionId;

                await Clients.Client(connectionId).SendAsync("ReceiveInvite", Context.UserIdentifier ?? Context.ConnectionId);

                _logger.LogInformation($"Invite sent to {connectionId} for player {targetPlayerId}");
            }
            else
            {
                await Clients.Caller.SendAsync("PlayerNotAvailable", targetPlayerId);
            }
        }


        /// <summary>
        /// Accepts a previously received game invitation.
        /// If the invitation is valid and pending, notifies the inviter that the invite has been accepted,
        /// initializes a new chess game board for both players using MediatR,
        /// and notifies both clients that the game has started.
        /// If the invitation is not found or invalid, notifies the caller that no pending invite exists.
        /// </summary>
        /// <param name="fromPlayerId">The unique identifier of the player who sent the invitation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AcceptInvite(string fromPlayerId)
        {

            if (_connections.TryGetValue(fromPlayerId, out var connectionId) &&
                _pendingInvites.TryRemove(Context.UserIdentifier ?? Context.ConnectionId, out var inviterConnectionId) &&
                inviterConnectionId == connectionId)
            {
                await Clients.Client(connectionId).SendAsync("InviteAccepted", Context.UserIdentifier ?? Context.ConnectionId);

                var command = new BoardInitializeCommand<
                    IRequestTypes<BoardInitializeRequestDTO>,
                    IResponseTypes<BoardInitializeResponseDTO, ChessGameResponseMessage>>(
                    new ChessGameRequest<BoardInitializeRequestDTO>(
                        new BoardInitializeRequestDTO
                        {
                            Player1Id = fromPlayerId,
                            Player2Id = Context.UserIdentifier ?? Context.ConnectionId
                        }));
                var result = await _mediator.Send(command);
                if (!result.IsSuccess)
                {
                    await Clients.Clients(new[] { connectionId, Context.ConnectionId }).SendAsync("GameInitializationFailed", result);
                    return;
                }

                await Clients.Clients(new[] { connectionId, Context.ConnectionId }).SendAsync("GameInitialized", result);


                await Clients.Clients(new[] { connectionId, Context.ConnectionId }).SendAsync("GameInitialized", "Game started!");
                _logger.LogInformation($"Invite accepted by {Context.ConnectionId} from player {fromPlayerId}");
            }
            else
            {
                await Clients.Caller.SendAsync("NoPendingInvite", fromPlayerId);
            }
        }


        /// <summary>
        /// Handles a player's move submission during an active game.
        /// Sends the move request to the mediator for validation and processing.
        /// If the move is valid, notifies the client that the move was made successfully;
        /// otherwise, notifies the client that the move failed.
        /// </summary>
        /// <param name="move">The move details submitted by the player, including source and destination positions.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task MakeMove(SubmitMoveRequestDTO move)
        {
            var command = new SubmitMoveCommand<
                IRequestTypes<SubmitMoveRequestDTO>,
                IResponseTypes<SubmitMoveResponseDTO, ChessGameResponseMessage>>(
                new ChessGameRequest<SubmitMoveRequestDTO>(move));
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                await Clients.Caller.SendAsync("MoveFailed", result);
                return;
            }
            await Clients.Caller.SendAsync("MoveMade", result);
            _logger.LogInformation($"Move made by {Context.ConnectionId}");
        }


        /// <summary>
        /// Allows a player to decline a received game invitation.
        /// If a valid pending invitation exists, notifies the inviter that the invite was declined
        /// and removes it from the pending invites list.
        /// Otherwise, notifies the caller that no pending invitation exists from the specified player.
        /// </summary>
        /// <param name="fromPlayerId">The unique identifier of the player who sent the invitation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeclineInvite(string fromPlayerId)
        {
            if (_connections.TryGetValue(fromPlayerId, out var connectionId) &&
                _pendingInvites.TryRemove(Context.UserIdentifier ?? Context.ConnectionId, out var inviterConnectionId) &&
                inviterConnectionId == connectionId)
            {
                await Clients.Client(connectionId).SendAsync("InviteDeclined", Context.UserIdentifier ?? Context.ConnectionId);
                _logger.LogInformation($"Invite declined by {Context.ConnectionId} from player {fromPlayerId}");
            }
            else
            {
                await Clients.Caller.SendAsync("NoPendingInvite", fromPlayerId);
            }
        }


        /// <summary>
        /// Cancels a previously sent game invitation.
        /// If a valid pending invitation exists for the target player,
        /// removes it from the pending invites list and notifies the target player
        /// that the invitation has been cancelled.
        /// Otherwise, notifies the caller that no such pending invitation exists.
        /// </summary>
        /// <param name="targetPlayerId">The unique identifier of the player whose invitation is being cancelled.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CancelInvite(string targetPlayerId)
        {
            if (_connections.TryGetValue(targetPlayerId, out var connectionId) &&
                _pendingInvites.TryRemove(targetPlayerId, out var inviterConnectionId) &&
                inviterConnectionId == Context.ConnectionId)
            {
                await Clients.Client(connectionId).SendAsync("InviteCancelled", Context.UserIdentifier ?? Context.ConnectionId);
                _logger.LogInformation($"Invite cancelled by {Context.ConnectionId} to player {targetPlayerId}");
            }
            else
            {
                await Clients.Caller.SendAsync("NoPendingInvite", targetPlayerId);
            }
        }
    }
}
