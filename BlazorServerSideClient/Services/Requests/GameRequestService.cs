using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using SharedResources.ChessGameResource.Enums.Users;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class GameRequestService(
        SignalRService signalRService,
        IConnectionHandlerService connectionHandlerService,
        JSRunetimeService jsRunetimeService)
        : IGameRequestService
    {
        public async Task<ResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(
            GetONlinePlayersRequestDTO getOnlinePlayersRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnection();
            var allGamersResult =
                await hubConnection.InvokeAsync<ResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>>(
                    "GetOnlinePlayersAsync", getOnlinePlayersRequestDto);

            if (!allGamersResult.IsSuccess) return allGamersResult;
            foreach (var guidAndConnections in allGamersResult.Data.OnlinePlayers)
                connectionHandlerService.OnlinePlayersUpdated?.Invoke(OnlinePlayerChangeType.Added,
                    guidAndConnections);

            return allGamersResult!;
        }


        public async Task<ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>> RequestTrainingGameAsync(
            TrainingGameRequestDTO trainingGameRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnection();
            return await hubConnection.InvokeAsync<ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>>(
                "RequestTrainingGameAsync", trainingGameRequestDto);
        }

        public async Task<ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(
            SendGameStateReqeustDTO gameStateReqeustDto)
        {
            var hubConnection = await signalRService.GetHubConnection();
            return await hubConnection.InvokeAsync<ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>>(
                "SendGameStateAsync", gameStateReqeustDto);
        }

        public async Task ClearGameAsync(Guid gameId)
        {
            var hubConnection = await (signalRService.GetHubConnection());
            await hubConnection.InvokeAsync<ResponseDTO<UserConnectionDTO, ChessGameResponseMessage>>("ClearGameAsync",
                gameId);
        }

        public async Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(
            MoveRequestDTO sendMoveConnectionRequestDto)
        {
            var hubConnection = await (signalRService.GetHubConnection());
            return await hubConnection.InvokeAsync<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>(
                "SendMoveAsync", sendMoveConnectionRequestDto);
        }

        public async Task<ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(
            ClickRequestDTO sendClickConnectionRequestDto)
        {
            var hubConnection = await (signalRService.GetHubConnection());
            return await hubConnection.InvokeAsync<ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>>(
                "SendClickAsync", sendClickConnectionRequestDto);
        }


        public async Task<bool> SendIsSameFigureClickedAsync(Position selectedPosition, Position currentPosition,
            Guid gameId)
        {
            var hubConnection = await (signalRService.GetHubConnection());
            return await hubConnection.InvokeAsync<bool>("SendIsSameFigureClickedAsync", selectedPosition,
                currentPosition, gameId);
        }
    }
}