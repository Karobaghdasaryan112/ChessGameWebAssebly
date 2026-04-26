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
using SharedResources.ChessGameResource.Enums.Users;

namespace BlazorServerSideClient.Services;

public sealed class SignalRService(
    IConnectionHandlerService connectionHandler,
    IInvitationHandlerService invitationHandler,
    IGameHandlerService gameHandler,
    AuthenticationStateProvider authStateProvider,
    ILogger<SignalRService> logger)

{
    private HubConnection? _hubConnection;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private CancellationTokenSource? _pingLoopCancellation;


    public async Task<HubConnection> GetHubConnectionAsync()
    {
        if (_hubConnection?.State is HubConnectionState.Connected or HubConnectionState.Reconnecting or HubConnectionState.Connecting)
        {
            return _hubConnection;
        }

        await _semaphore.WaitAsync();
        try
        {
            if (_hubConnection == null)
            {
                _hubConnection = BuildHubConnection();
                RegisterHandlers(_hubConnection);
            }

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }

            _pingLoopCancellation?.Cancel();
            _pingLoopCancellation = new CancellationTokenSource();
            _ = StartPingLoopAsync(_pingLoopCancellation.Token);

            await NotifyServerOfConnectionAsync(_hubConnection);

            return _hubConnection;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize SignalR connection.");
            _hubConnection = null;
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }


    private HubConnection BuildHubConnection()
    {
        return new HubConnectionBuilder()
            .WithUrl(BasePaths.baseUrlHub)
            .WithAutomaticReconnect()
            .WithKeepAliveInterval(TimeSpan.FromSeconds(60))
            .WithServerTimeout(TimeSpan.FromSeconds(60))
            .Build();
    }


    private async Task NotifyServerOfConnectionAsync(HubConnection connection)
    {
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = user.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        {
            logger.LogWarning("SignalR attempt made by unauthenticated or invalid user.");
            return;
        }

        var request = new AddUserConnectionRequestDTO
        {
            userGuid = userGuid,
            userConnection = new UserConnectionDTO
            {
                ConnectionId = connection.ConnectionId ?? string.Empty,
                UserName = userName ?? "Unknown"
            }
        };

        await connection.SendAsync("AddConnectionAsync", request);
    }


    private void RegisterHandlers(HubConnection connection)
    {
        connection.On<KeyValuePair<Guid, UserConnectionDTO>, OnlinePlayerChangeType>(
            "ReceiveUpdatedUsers",
            connectionHandler.ReceiveUpdatedUsers);

        connection.On<UserConnectionDTO, Guid, UserConnectionDTO, Guid>(
            "ReceiveInvite",
            invitationHandler.ReceiveInvite);

        connection.On<UserConnectionDTO, Guid, UserConnectionDTO, Guid, Guid>(
            "InviteAcceptedAsync",
            invitationHandler.InviteAcceptedAsync);

        connection.On<ResponseDTO<ReceivePlayersResponseDTO, ChessGameResponseMessage>>(
            "ReseivePlayersAsync",
            gameHandler.ReseivePlayersAsync);

        connection.On<ResponseDTO<BoardStateResponseDTO, ChessGameResponseMessage>>(
            "ReceiveBoardUpdateAsync",
            gameHandler.ReceiveBoardUpdateAsync);

        connection.On<KeyValuePair<Guid, UserConnectionDTO>>(
            "DisconnectedNotification",
            async (data) =>
            {
                await gameHandler.NotifyOpponentUserDisconnected(data);
                connectionHandler.DisconnectedNotification(data);
            });

        connection.On<string>("OpponentLeftWinNotification", async (leavingPlayerName) =>
        {
            await gameHandler.NotifyOpponentLeftWinAsync(leavingPlayerName);
        });

        connection.On("ForceNavigateToDashboard", async () =>
        {
            await Task.Delay(100);
            await gameHandler.RedirectToDashboardAsync();
        });

        connection.Closed += async (error) =>
        {
            if (error != null) logger.LogError(error, "SignalR connection closed with error.");
            _pingLoopCancellation?.Cancel();
            await Task.CompletedTask;
        };
        
        connection.Reconnected += async (_) => await NotifyServerOfConnectionAsync(connection);
    }

    private async Task StartPingLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                if (_hubConnection?.State == HubConnectionState.Connected)
                {
                    await _hubConnection.SendAsync("Ping", cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("SignalR connection closed.");
            throw;
            }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "SignalR ping loop stopped unexpectedly.");
        }
    }
}