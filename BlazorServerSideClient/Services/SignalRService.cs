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


    public async Task<HubConnection> GetHubConnectionAsync()
    {
        
        if (_hubConnection?.State is HubConnectionState.Connected or HubConnectionState.Reconnecting)
        {
            return _hubConnection;
        }

        await _semaphore.WaitAsync();
        try
        {
            if (_hubConnection != null) return _hubConnection;
            _hubConnection = BuildHubConnection();
            
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    if (_hubConnection?.State == HubConnectionState.Connected)
                    {
                        await _hubConnection.SendAsync("Ping");
                    }
                    await Task.Delay(10000);
                }
            });

            RegisterHandlers(_hubConnection);

            await _hubConnection.StartAsync();
            await NotifyServerOfConnectionAsync(_hubConnection);

            return _hubConnection;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize SignalR connection.");
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

        connection.Closed += async (error) =>
        {
            if (error != null) logger.LogError(error, "SignalR connection closed with error.");
            await Task.CompletedTask;
        };
        
        connection.Reconnected += async (_) => await NotifyServerOfConnectionAsync(connection);
    }
}