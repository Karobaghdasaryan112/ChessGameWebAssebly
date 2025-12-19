using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;
using BoardInitializeRequestDTO = SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs.BoardInitializeRequestDTO;

namespace ChessGame.Core.Services.Contracts.BoardServices
{
    public interface IBoardService
    {
        Task<ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>> InitializeBoardAsync(ConnectionRequestDTO<BoardInitializeRequestDTO> connectionRequestDto);

        Task<ResponseDTO<SavePositionsResponseDTO, ChessGameResponseMessage>> SavePositionsAsync(
            ConnectionRequestDTO<SavePositionsRequestDTO> savePositionsRequest);

        Task<ResponseDTO<SaveGameEventAndWinnerResponseDTO, ChessGameResponseMessage>> SaveGameEventAndWinnerAsync(
            ConnectionRequestDTO<SaveGameEventAndWinnerRequestDTO> connectionRequestDTO);
        Task<ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>>
     GetGameHistoryAsync(ConnectionRequestDTO<GetGameHistoryRequestDTO> requestDTO);
    }
}
