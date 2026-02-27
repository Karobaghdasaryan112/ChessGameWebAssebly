using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace BlazorServerSideClient.Contracts.Requests
{
    public interface IGameRequestService
    {
        Task<ResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(GetONlinePlayersRequestDTO getOnlinePlayersRequestDto);
        Task<ResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(SendGameStateReqeustDTO gameStateReqeustDto);
        Task ClearGameAsync(Guid gameId);
        Task<ResponseDTO<TrainingGameResponseDTO, ChessGameResponseMessage>> RequestTrainingGameAsync(TrainingGameRequestDTO trainingGameRequestDto);
        Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(MoveRequestDTO sendMoveConnectionRequestDto);    
        Task<bool> SendIsSameFigureClickedAsync(Position selectedPosition, Position currentPosition, Guid gameId);
        Task<ResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(ClickRequestDTO sendClickConnectionRequestDto);
    }
}
