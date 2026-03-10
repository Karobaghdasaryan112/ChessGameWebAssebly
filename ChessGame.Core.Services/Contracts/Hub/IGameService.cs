using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.Contracts.Hub
{
    public interface IGameService
    {
        Task<ResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(GetONlinePlayersRequestDTO connectionRequestDTO);
        Task<ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(SendGameStateReqeustDTO gameStateReqeustDTO);
        Task ClearGameAsync(Guid gameId);
        Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(
            MoveRequestDTO sendMoveConnectionRequestDTO);
        Task<bool> SendIsSameFigureClickedAsync((Position selectedPosition, Position currentPosition, Guid gameId) data);
        Task<ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(ClickRequestDTO sendClickConnectionRequestDTO);
        Task<ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>> RequestTrainingGameAsync(
           TrainingGameRequestDTO trainingGameRequestDTO);
    }
}
