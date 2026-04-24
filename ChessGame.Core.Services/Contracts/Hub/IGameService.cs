using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IGameService
    {
        Task<PipeLineResponse<GetOnlinePlayersResponseDTO>>
                    GetOnlinePlayersAsync(PipeLineRequest<GetONlinePlayersRequestDTO> connectionRequestDTO);
        Task<ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(SendGameStateReqeustDTO gameStateReqeustDTO);
        Task ClearGameAsync(Guid gameId);

        Task<PipeLineResponse<MoveResponseDTO>> SendMoveAsync(
            MoveRequestDTO sendMoveConnectionRequestDTO);
        Task<bool> SendIsSameFigureClickedAsync(SameFigureRequest sameFigureRequest);
        Task<ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(ClickRequestDTO sendClickConnectionRequestDTO);
        Task<ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>> RequestTrainingGameAsync(
           TrainingGameRequestDTO trainingGameRequestDTO);
    }
}
