using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Contracts.Requests;
using BlazorServerSideClient.Extensions;
using SharedResources.ChessGameResource.Enums.Users;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;

namespace BlazorServerSideClient.Services.Requests
{
    public class GameRequestService(
        JSRunetimeService JSRunetimeService,
        SignalRService signalRService,
        IConnectionHandlerService connectionHandlerService)
        : IGameRequestService
    {
        public async Task<PipeLineResponse<GetOnlinePlayersResponseDTO>> GetOnlinePlayersAsync(
            PipeLineRequest<GetONlinePlayersRequestDTO> getOnlinePlayersRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            var allGamersResult =
                await hubConnection.SafeInvokeAsync<GetONlinePlayersRequestDTO, GetOnlinePlayersResponseDTO>(
                    "GetOnlinePlayersAsync", getOnlinePlayersRequestDto.Request, JSRunetimeService);


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

            return await hubConnection.SafeInvokeAsync<TrainingGameRequestDTO, TrainingGameResponseDTO>
                       ("RequestTrainingGameAsync", trainingGameRequestDto.Request, JSRunetimeService) ??
                   PipeLineResponse<TrainingGameResponseDTO>.Emoty;
        }

        public async Task<PipeLineResponse<SendGameStateResponseDTO>> SendGameStateAsync(
            PipeLineRequest<SendGameStateReqeustDTO> gameStateRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.SafeInvokeAsync<SendGameStateReqeustDTO, SendGameStateResponseDTO>(
                       "SendGameStateAsync", gameStateRequestDto.Request, JSRunetimeService) ??
                   PipeLineResponse<SendGameStateResponseDTO>.Emoty;
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

            return await hubConnection.SafeInvokeAsync<MoveRequestDTO, MoveResponseDTO>("SendMoveAsync",
                sendMoveConnectionRequestDto.Request, JSRunetimeService) ?? PipeLineResponse<MoveResponseDTO>.Emoty;
        }

        public async Task<PipeLineResponse<ClickResponseDTO>> SendClickAsync(
            PipeLineRequest<ClickRequestDTO> sendClickConnectionRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.SafeInvokeAsync<ClickRequestDTO, ClickResponseDTO>("SendClickAsync",
                sendClickConnectionRequestDto.Request, JSRunetimeService) ?? PipeLineResponse<ClickResponseDTO>.Emoty;
        }

        //TO DO:
        public async Task<PipeLineResponse<SameFigureResposneDTO>> SendIsSameFigureClickedAsync(
            PipeLineRequest<SameFigureRequest> sendIsSameFigureClickedConnectionRequestDto)
        {
            var hubConnection = await signalRService.GetHubConnectionAsync();

            return await hubConnection.SafeInvokeAsync<SameFigureRequest, SameFigureResposneDTO>(
                       "SendIsSameFigureClickedAsync",
                       sendIsSameFigureClickedConnectionRequestDto.Request, JSRunetimeService) ??
                   PipeLineResponse<SameFigureResposneDTO>.Emoty;
        }
    }
}