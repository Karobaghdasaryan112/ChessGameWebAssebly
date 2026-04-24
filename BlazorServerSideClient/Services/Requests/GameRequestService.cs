using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Contracts.Requests;
using BlazorServerSideClient.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.ChessGameResource.Enums.Users;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;

namespace BlazorServerSideClient.Services.Requests
{
    public class GameRequestService(
        JSRunetimeService jsRuneTimeService,
        SignalRService signalRService,
        IConnectionHandlerService connectionHandlerService)
        : IGameRequestService
    {
        public async Task<PipeLineResponse<GetOnlinePlayersResponseDTO>> GetOnlinePlayersAsync(PipeLineRequest<GetONlinePlayersRequestDTO> getOnlinePlayersRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            var allGamersResult =
                await hubConnection.SafeInvokeAsync<GetONlinePlayersRequestDTO, GetOnlinePlayersResponseDTO>(
                    "GetOnlinePlayersAsync", getOnlinePlayersRequestDto.Request, jsRuneTimeService);



            if (!allGamersResult.Response.IsSuccess) return allGamersResult;
            foreach (var guidAndConnections in allGamersResult.Response.Data.OnlinePlayers)
                connectionHandlerService.OnlinePlayersUpdated?.Invoke(OnlinePlayerChangeType.Added,
                    guidAndConnections);

            return allGamersResult!;
        }


        public async Task<PipeLineResponse<TrainingGameResponseDTO>> RequestTrainingGameAsync(
            PipeLineRequest<TrainingGameRequestDTO> trainingGameRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();
            return await hubConnection.InvokeAsync<PipeLineResponse<TrainingGameResponseDTO>>(
                "RequestTrainingGameAsync", trainingGameRequestDto);
        }

        public async Task<PipeLineResponse<SendGameStateResponseDTO>> SendGameStateAsync(
            PipeLineRequest<SendGameStateReqeustDTO> gameStateRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();
            return await hubConnection.InvokeAsync<PipeLineResponse<SendGameStateResponseDTO>>(
                "SendGameStateAsync", gameStateRequestDto);
        }


        //TO DO:
        //public async Task<PipeLineResponse<>> ClearGameAsync(Guid gameId)
        //{
        //    var hubConnection = await signalRService.GetHubConnectionAsync();

        //    return await hubConnection.InvokeAsync<PipeLineResponse<UserConnectionDTO>>("ClearGameAsync",
        //        gameId);
        //}

        public async Task<PipeLineResponse<MoveResponseDTO>> SendMoveAsync(
            PipeLineRequest<MoveRequestDTO> sendMoveConnectionRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.InvokeAsync<PipeLineResponse<MoveResponseDTO>>(
                "SendMoveAsync", sendMoveConnectionRequestDto);
        }

        public async Task<PipeLineResponse<ClickResponseDTO>> SendClickAsync(
            PipeLineRequest<ClickRequestDTO> sendClickConnectionRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.InvokeAsync<PipeLineResponse<ClickResponseDTO>>(
                "SendClickAsync", sendClickConnectionRequestDto);
        }


        //TO DO:
        public async Task<PipeLineResponse<object>> SendIsSameFigureClickedAsync(Position selectedPosition, Position currentPosition,
            Guid gameId)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.InvokeAsync<PipeLineResponse<object>>("SendIsSameFigureClickedAsync", selectedPosition,
                currentPosition, gameId);
        }
    }
}