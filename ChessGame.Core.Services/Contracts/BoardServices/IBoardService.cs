using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using BoardInitializeRequestDTO = SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs.BoardInitializeRequestDTO;

namespace ChessGame.Core.Services.Contracts.BoardServices
{
    public interface IBoardService
    {
        Task<ConnectionResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>> InitializeBoardAsync(ConnectionRequestDTO<BoardInitializeRequestDTO> connectionRequestDto);
        Task<ConnectionResponseDTO<SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs.SubmitMoveResponseDTO, ChessGameResponseMessage>> SubmitMoveAsync(SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs.SubmitMoveRequestDTO  submitMoveRequestDto);
        Task<ConnectionResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>> CanClick(ConnectionRequestDTO<CanClickRequestDTO> connectionRequestDto);
        Task<bool> IsKingCheckedAsync(Board currentBoard,Turn chosenColor);
        Task<bool> IsKingMateAsync(Board? currentBoard, Guid gameId, Turn chosenColor);
        void ResetEventableBlocks(Board gameState);
    }
}
