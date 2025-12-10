using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Repositories;
using Microsoft.Extensions.Logging;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Net;
using BoardInitializeRequestDTO = SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs.BoardInitializeRequestDTO;

namespace ChessGame.Core.Services.Services.BoardService
{
    public class BoardService(
        ILogger<BoardService> logger,
        IChessGameRepository chessGameRepository,
        IChessGameHistoryRepository chessGameHistoryRepository)
        : IBoardService
    {
        public async Task<ConnectionResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>>
            InitializeBoardAsync(ConnectionRequestDTO<BoardInitializeRequestDTO> connectionRequestDto)
        {

            var isCreated = await chessGameRepository.CreateGame(
                connectionRequestDto.Data.Player1Id,
                connectionRequestDto.Data.Player2Id,

                connectionRequestDto.Data.Player1Name,
                connectionRequestDto.Data.Player2Name,

                connectionRequestDto.Data.Player1Time.Minutes,
                connectionRequestDto.Data.Player2Time.Minutes);

            if (!isCreated)
            {
                logger.LogError("Failed to create a new game between {Player1} and {Player2}",
                    connectionRequestDto.Data.Player1Id, connectionRequestDto.Data.Player2Id);
                return ConnectionResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new BoardInitializeResponseDTO()
                    {
                        GameId = Guid.Empty
                    }, ChessGameResponseMessage.GameCreationFailed, HttpStatusCode.BadRequest,
                    [
                        "Failed to create a new game between {Player1} and {Player2}",
                        connectionRequestDto.Data.Player1Id.ToString(), connectionRequestDto.Data.Player2Id.ToString()
                    ]);
            }

            var gameId = await chessGameRepository.GetGameIdByPlayers(connectionRequestDto.Data.Player1Id,
                connectionRequestDto.Data.Player2Id);
            if (gameId == Guid.Empty)
            {
                logger.LogError("Failed to retrieve game ID for players {Player1} and {Player2}",
                    connectionRequestDto.Data.Player1Id, connectionRequestDto.Data.Player2Id);
                return ConnectionResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new BoardInitializeResponseDTO()
                    {
                        GameId = Guid.Empty
                    }, ChessGameResponseMessage.GameCreationFailed, HttpStatusCode.BadRequest,
                    [
                        "Failed to retrieve game ID for players {Player1} and {Player2}",
                        connectionRequestDto.Data.Player1Id.ToString(), connectionRequestDto.Data.Player2Id.ToString()
                    ]);
            }
            else
                logger.LogInformation("Game successfully created between {Player1} and {Player2}",
                    connectionRequestDto.Data.Player1Id, connectionRequestDto.Data.Player2Id);

            return ConnectionResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new BoardInitializeResponseDTO()
                {
                    GameId = gameId
                },
                ChessGameResponseMessage.GameCreated,
                HttpStatusCode.Created);
        }

    }
}
