using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Requests
{
    public interface IGameRequestService
    {
        Task<PipeLineResponse<GetOnlinePlayersResponseDTO>> GetOnlinePlayersAsync(
            PipeLineRequest<GetONlinePlayersRequestDTO> getOnlinePlayersRequestDto);

        Task<PipeLineResponse<SendGameStateResponseDTO>> SendGameStateAsync(
            PipeLineRequest<SendGameStateReqeustDTO> gameStateRequestDto);

        //Task<PipeLineResponse<object>> ClearGameAsync(Guid gameId);

        Task<PipeLineResponse<TrainingGameResponseDTO>> RequestTrainingGameAsync(
            PipeLineRequest<TrainingGameRequestDTO> trainingGameRequestDto);

        Task<PipeLineResponse<MoveResponseDTO>> SendMoveAsync(
            PipeLineRequest<MoveRequestDTO> sendMoveConnectionRequestDto);

        Task<PipeLineResponse<object>> SendIsSameFigureClickedAsync(Position selectedPosition, Position currentPosition,
            Guid gameId);

        Task<PipeLineResponse<ClickResponseDTO>> SendClickAsync(
            PipeLineRequest<ClickRequestDTO> sendClickConnectionRequestDto);
    }
}
