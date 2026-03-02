using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Contracts.Requests;
using ChessGameBlazorClient.UI.Services;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Services.Requests
{
    public class GameRequestService(SignalRService signalRService, IConnectionHandlerService connectionHandlerService, JSRunetimeService jsRunetimeService)
        : IGameRequestService
    {
        public async Task<ResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(GetONlinePlayersRequestDTO getOnlinePlayersRequestDto)
        {

            var allGamersResult = await jsRunetimeService.
                SendAsync<
                    GetONlinePlayersRequestDTO,
                    ResponseDTO<
                        GetOnlinePlayersResponseDTO,
                        ChessGameResponseMessage>>(
                "GetOnlinePlayersAsync",
                getOnlinePlayersRequestDto);

            if (allGamersResult.IsSuccess)
                foreach (var guidAndConnections in allGamersResult.Data.OnlinePlayers)
                    connectionHandlerService.OnlinePlayersUpdated?.Invoke(guidAndConnections);

            return allGamersResult;
        }


        public Task<ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>> RequestTrainingGameAsync(TrainingGameRequestDTO trainingGameRequestDto)
         => jsRunetimeService.
            SendAsync<
                TrainingGameRequestDTO,
                ResponseDTO<
                    TrainingGameResponseDTO,
                    ChessGameResponseMessage>>(
             "RequestTrainingGameAsync",
             trainingGameRequestDto);


        public Task<ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(SendGameStateReqeustDTO gameStateReqeustDto)
            => jsRunetimeService.
                SendAsync<
                    SendGameStateReqeustDTO,
                    ResponseDTO<
                        SendGameStateResponseDTO,
                        ChessGameResponseMessage>>(
             "SendGameStateAsync",
             gameStateReqeustDto);

        public Task ClearGameAsync(Guid gameId)
            => jsRunetimeService.SendAsync<Guid, ResponseDTO<UserConnectionDTO, ChessGameResponseMessage>>("ClearGameAsync", gameId);


        public Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(MoveRequestDTO sendMoveConnectionRequestDto)
            => jsRunetimeService.
                SendAsync<
                    MoveRequestDTO,
                    ResponseDTO<
                        MoveResponseDTO,
                        ChessGameResponseMessage>>(
             "SendMoveAsync",
             sendMoveConnectionRequestDto);


        public Task<ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(ClickRequestDTO sendClickConnectionRequestDto)
            => jsRunetimeService.
                SendAsync<
                    ClickRequestDTO,
                    ResponseDTO<
                        ClickResponseDTO,
                        ChessGameResponseMessage>>(
             "SendClickAsync",
             sendClickConnectionRequestDto);


        public Task<bool> SendIsSameFigureClickedAsync(Position selectedPosition, Position currentPosition, Guid gameId)
            => jsRunetimeService.
                SendAsync<
                    (Position, Position, Guid),
                    bool>(
                "SendIsSameFigureClickedAsync",
                (selectedPosition, currentPosition, gameId));
    }
}