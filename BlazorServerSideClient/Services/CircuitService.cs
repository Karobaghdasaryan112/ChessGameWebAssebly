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

    public override async Task OnConnectionDownAsync(
        Circuit handler,
        CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(3000, cancellationToken);

            var removeUsersFromGameRequestDto = new RemoveUsersFromGameReqeustDTO
            {
                connectionId = null,
                CurerntPlayerGuid = GameCircuitState.UserId!,
                GameId = GameCircuitState.UserId,
                IsLeaveWebSite = true,

            };
            await gameService.LeaveGameAsync(removeUsersFromGameRequestDto);

            // var removeUserConnectionRequestDto = new RemoveUserConnectionRequestDTO
            // {
            //     connectionId = null,
            //     UserGuid = removeUsersFromGameRequestDto.CurerntPlayerGuid,
            //     GameId = removeUsersFromGameRequestDto.GameId
            // };

            // await connectionService.RemoveConnectionAsync(new PipeLineRequest<RemoveUserConnectionRequestDTO>
            // {
            //     Request = removeUserConnectionRequestDto
            // });
            logger.LogInformation("Blazor circuit connection DOWN: {CircuitId}", handler.Id);
            GameCircuitState.GameId = default;
        }
        catch (OperationCanceledException ex)
        {
            logger.LogInformation(ex, "connection down operation was cancelled: {CircuitId}", handler.Id);
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "unhandled exception was occured: {CircuitId}", handler.Id);
        }
    }


    public override async Task OnConnectionUpAsync(
        Circuit circuit,
        CancellationToken cancellationToken)
    {
        var connection = await signalRService.GetHubConnectionAsync();
        var addConnectionRequestDTO = new AddUserConnectionRequestDTO
        {
            connectionId = connection.ConnectionId!,
            userGuid = GameCircuitState.UserId,
            userConnection = GameCircuitState.UserConnectionDto
        };
        await connectionService.AddConnectionAsync(
            new PipeLineRequest<AddUserConnectionRequestDTO>(addConnectionRequestDTO));
        logger.LogInformation("Blazor circuit connection UP: {CircuitId}", circuit.Id);
    }

    public override Task OnCircuitClosedAsync(
        Circuit circuit,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Blazor circuit CLOSED: {CircuitId}", circuit.Id);
        return Task.CompletedTask;
    }
}