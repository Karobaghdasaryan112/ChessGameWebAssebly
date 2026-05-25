using BlazorServerSideClient.Contracts.Requests;
using BlazorServerSideClient.Data.GameModels;
using ChessGame.Core.Services.Contracts.Hub;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.Components.Server.Circuits;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.UserConnectionRequestDTOs;
using SharedResources.PipeLine.PipeLineContext;

namespace BlazorServerSideClient.Services;

sealed class GameCircuitHandler(
    ILogger<GameCircuitHandler> logger,
    SignalRService signalRService,
    IGameRequestService gameService,
    IConnectionReqeustService connectionService,
    GameCircuitState gameCircuitState) : CircuitHandler
{
    public GameCircuitState GameCircuitState = gameCircuitState;

    public override Task OnConnectionDownAsync(
        Circuit circuit,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }


    public override async Task OnConnectionUpAsync(
        Circuit circuit,
        CancellationToken cancellationToken)
    {
        try
        {
            var connection = await signalRService.GetHubConnectionAsync();

            if (connection.ConnectionId is null || GameCircuitState.UserId is null)
            {
                logger.LogWarning(
                    "Skipping AddConnection because ConnectionId or UserId is null. CircuitId: {CircuitId}",
                    circuit.Id);

                return;
            }

            var addConnectionRequestDTO = new AddUserConnectionRequestDTO
            {
                connectionId = connection.ConnectionId,
                userGuid = (Guid)GameCircuitState.UserId,
                userConnection = GameCircuitState.UserConnectionDto
            };

            await connectionService.AddConnectionAsync(
                new PipeLineRequest<AddUserConnectionRequestDTO>(addConnectionRequestDTO));

            logger.LogInformation("Blazor circuit connection UP: {CircuitId}", circuit.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception during connection up: {CircuitId}", circuit.Id);
        }
    }

    public override async Task OnCircuitClosedAsync(
        Circuit circuit,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Blazor circuit connection DOWN: {CircuitId}", circuit.Id);

            await Task.Delay(TimeSpan.FromSeconds(6), cancellationToken);

            if (GameCircuitState.UserId is null || GameCircuitState.GameId is null)
            {
                logger.LogWarning(
                    "Skipping disconnect cleanup because UserId or GameId is null. CircuitId: {CircuitId}, UserId: {UserId}, GameId: {GameId}",
                    circuit.Id,
                    GameCircuitState.UserId,
                    GameCircuitState.GameId);

                return;
            }

            var removeUsersFromGameRequestDto = new RemoveUsersFromGameReqeustDTO
            {
                connectionId = null,
                CurerntPlayerGuid = (Guid)GameCircuitState.UserId,
                GameId = (Guid)GameCircuitState.GameId,
                IsLeaveWebSite = true,
            };

            await gameService.LeaveGameAsync(removeUsersFromGameRequestDto);

            var removeUserConnectionRequestDto = new RemoveUserConnectionRequestDTO
            {
                connectionId = null,
                UserGuid = (Guid)GameCircuitState.UserId,
                GameId = (Guid)GameCircuitState.GameId
            };

            GameCircuitState.GameId = default;

            logger.LogInformation("Disconnect cleanup completed. CircuitId: {CircuitId}", circuit.Id);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Connection down cleanup was cancelled: {CircuitId}", circuit.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception during connection down cleanup: {CircuitId}", circuit.Id);
        }
    }
}