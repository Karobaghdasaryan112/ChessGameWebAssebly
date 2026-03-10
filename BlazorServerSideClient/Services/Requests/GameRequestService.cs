using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
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
            var signalRModel = await signalRService.GetHubConnection();

            var allGamersResult = await jsRunetimeService.SendAsync<
                GetONlinePlayersRequestDTO,
                ResponseDTO<
                    GetOnlinePlayersResponseDTO,
                    ChessGameResponseMessage>>(
                "GetOnlinePlayersAsync",
                getOnlinePlayersRequestDto);

            if (allGamersResult.IsSuccess)
                foreach (var guidAndConnections in allGamersResult.Data.OnlinePlayers)
                    connectionHandlerService.OnlinePlayersUpdated?.Invoke(OnlinePlayerChangeType.Added,
                        guidAndConnections);

            return allGamersResult;
        }


        public async Task<ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>> RequestTrainingGameAsync(
            TrainingGameRequestDTO trainingGameRequestDto)
        {
            var signalRModel = await signalRService.GetHubConnection();
            return await jsRunetimeService.SendAsync<
                TrainingGameRequestDTO,
                ResponseDTO<
                    TrainingGameResponseDTO,
                    ChessGameResponseMessage>>(
                "RequestTrainingGameAsync",
                trainingGameRequestDto);
        }


        public async Task<ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>>
            SendGameStateAsync(SendGameStateReqeustDTO gameStateReqeustDto)
        {
            var signalRModel = await signalRService.GetHubConnection();

            return await jsRunetimeService.SendAsync<
                SendGameStateReqeustDTO,
                ResponseDTO<
                    SendGameStateResponseDTO,
                    ChessGameResponseMessage>>(
                "SendGameStateAsync",
                gameStateReqeustDto);
        }

        public async Task
            ClearGameAsync(Guid gameId)
        {
            var signalRModel = await signalRService.GetHubConnection();

            await jsRunetimeService.SendAsync<
                Guid,
                ResponseDTO<
                    UserConnectionDTO,
                    ChessGameResponseMessage>>(
                "ClearGameAsync",
                gameId);
        }


        public async Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>
            SendMoveAsync(MoveRequestDTO sendMoveConnectionRequestDto)
        {
            var signalRModel = await signalRService.GetHubConnection();

            return await jsRunetimeService.SendAsync<
                MoveRequestDTO,
                ResponseDTO<
                    MoveResponseDTO,
                    ChessGameResponseMessage>>(
                "SendMoveAsync",
                sendMoveConnectionRequestDto);
        }


        public async Task<ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>>
            SendClickAsync(ClickRequestDTO sendClickConnectionRequestDto)
        {
            var signalRModel = await signalRService.GetHubConnection();

            return await jsRunetimeService.SendAsync<
                ClickRequestDTO,
                ResponseDTO<
                    ClickResponseDTO,
                    ChessGameResponseMessage>>(
                "SendClickAsync",
                sendClickConnectionRequestDto);
        }


        public async Task<bool> SendIsSameFigureClickedAsync(SameFigureRequest sameFigureRequest)
        {
            var signalRModel = await signalRService.GetHubConnection();
            return await jsRunetimeService.SendAsync<
                SameFigureRequest,
                bool>(
                "SendIsSameFigureClickedAsync", sameFigureRequest);
        }
    }
}