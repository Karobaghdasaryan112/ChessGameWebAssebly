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
        Task<ConnectionResponseDTO<GetOnlinePlayersResponseDTO, ChessGameResponseMessage>> GetOnlinePlayersAsync(ConnectionRequestDTO<GetONlinePlayersRequestDTO> connectionRequestDTO);
        Task<ConnectionResponseDTO<SendGameStateResponseDTO, ChessGameResponseMessage>> SendGameStateAsync(ConnectionRequestDTO<SendGameStateReqeustDTO> gameStateReqeustDTO);
        Task ClearGameAsync(Guid gameId);
        Task<ConnectionResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> SendMoveAsync(
            ConnectionRequestDTO<MoveRequestDTO> sendMoveConnectionRequestDTO);
        Task<bool> SendIsSameFigureClickedAsync(Position selectedPosition, Position currentPosition, Guid gameId);
        Task<ConnectionResponseDTO<ClickResponseDTO, ChessGameResponseMessage>> SendClickAsync(ConnectionRequestDTO<ClickRequestDTO> sendClickConnectionRequestDTO);
    }
}
