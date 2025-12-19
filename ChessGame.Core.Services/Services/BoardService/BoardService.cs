using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Domain.Domain.Entities;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;
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
        public async Task<ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>>
            InitializeBoardAsync(ConnectionRequestDTO<BoardInitializeRequestDTO> connectionRequestDto)
        {

            var isCreated = await chessGameRepository.CreateGame(
                connectionRequestDto.Data.Player1Id,
                connectionRequestDto.Data.Player2Id,

                GameEvent.Start,

                connectionRequestDto.Data.Player1Name,
                connectionRequestDto.Data.Player2Name,

                (int)connectionRequestDto.Data.Player1Time.TotalSeconds,
                (int)connectionRequestDto.Data.Player2Time.TotalSeconds);


            if (!isCreated)
            {
                logger.LogError("Failed to create a new game between {Player1} and {Player2}",
                    connectionRequestDto.Data.Player1Id, connectionRequestDto.Data.Player2Id);
                return ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
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
                return ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
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

            return ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new BoardInitializeResponseDTO()
                {
                    GameId = gameId
                },
                ChessGameResponseMessage.GameCreated,
                HttpStatusCode.Created);
        }

        public async Task<ResponseDTO<SaveGameEventAndWinnerResponseDTO,
        ChessGameResponseMessage>>
        SaveGameEventAndWinnerAsync(
            ConnectionRequestDTO<SaveGameEventAndWinnerRequestDTO> connectionRequestDTO)
        {
            logger.LogInformation(
                "SaveGameEventAndWinnerAsync started. Request: {@Request}",
                connectionRequestDTO);

            if (connectionRequestDTO == null || connectionRequestDTO.Data == null)
            {
                logger.LogWarning("Request or Request.Data is null");

                return ResponseDTO<
                    SaveGameEventAndWinnerResponseDTO,
                    ChessGameResponseMessage>
                .CreateErrorResponse(
                    new SaveGameEventAndWinnerResponseDTO { IsSaved = false },
                    ChessGameResponseMessage.InvalidData,
                    HttpStatusCode.BadRequest);
            }

            if (connectionRequestDTO.Data.GameId == Guid.Empty ||
                connectionRequestDTO.Data.WinnerPlayerGuid == Guid.Empty)
            {
                logger.LogWarning(
                    "Invalid GameId or WinnerPlayerGuid. GameId: {GameId}, Winner: {Winner}",
                    connectionRequestDTO.Data.GameId,
                    connectionRequestDTO.Data.WinnerPlayerGuid);

                return ResponseDTO<
                    SaveGameEventAndWinnerResponseDTO,
                    ChessGameResponseMessage>
                .CreateErrorResponse(
                    new SaveGameEventAndWinnerResponseDTO { IsSaved = false },
                    ChessGameResponseMessage.InvalidData,
                    HttpStatusCode.BadRequest);
            }

            try
            {
                var isSaved = await chessGameRepository.SaveGameResult(
                    connectionRequestDTO.Data.WinnerPlayerGuid,
                    connectionRequestDTO.Data.GameId);

                if (!isSaved)
                {
                    logger.LogWarning(
                        "SaveGameResult returned false. GameId: {GameId}, Winner: {Winner}",
                        connectionRequestDTO.Data.GameId,
                        connectionRequestDTO.Data.WinnerPlayerGuid);

                    return ResponseDTO<
                        SaveGameEventAndWinnerResponseDTO,
                        ChessGameResponseMessage>
                    .CreateErrorResponse(
                        new SaveGameEventAndWinnerResponseDTO { IsSaved = false },
                        ChessGameResponseMessage.InternalServerError,
                        HttpStatusCode.InternalServerError);
                }

                logger.LogInformation(
                    "Game result successfully saved. GameId: {GameId}, Winner: {Winner}",
                    connectionRequestDTO.Data.GameId,
                    connectionRequestDTO.Data.WinnerPlayerGuid);

                return ResponseDTO<
                    SaveGameEventAndWinnerResponseDTO,
                    ChessGameResponseMessage>
                .CreateSuccessResponse(
                    new SaveGameEventAndWinnerResponseDTO { IsSaved = true },
                    ChessGameResponseMessage.MoveSuccessful,
                    HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Exception while saving game result. GameId: {GameId}",
                    connectionRequestDTO?.Data?.GameId);

                return ResponseDTO<
                    SaveGameEventAndWinnerResponseDTO,
                    ChessGameResponseMessage>
                .CreateErrorResponse(
                    new SaveGameEventAndWinnerResponseDTO { IsSaved = false },
                    ChessGameResponseMessage.InternalServerError,
                    HttpStatusCode.InternalServerError);
            }
        }


        public async Task<ResponseDTO<SavePositionsResponseDTO, ChessGameResponseMessage>> SavePositionsAsync(
            ConnectionRequestDTO<SavePositionsRequestDTO> savePositionsRequest)
        {
            var chessGameHistoryModel = new ChessGameHistory()
            {
                FEN = savePositionsRequest.Data.FEN,
                GameId = savePositionsRequest.Data.GameId,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };
            var isGameStateSaved = await chessGameHistoryRepository.SaveGameStateAsync(chessGameHistoryModel);

            return isGameStateSaved
                ? ResponseDTO<SavePositionsResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    new SavePositionsResponseDTO()
                    {
                        IsSave = true
                    },
                    ChessGameResponseMessage.GameCreated,
                    HttpStatusCode.OK)

                : ResponseDTO<SavePositionsResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new SavePositionsResponseDTO()
                    {
                        IsSave = false
                    },
                    ChessGameResponseMessage.InternalServerError, HttpStatusCode.InternalServerError,
                    ["Fail to Save Board State into DB"]);
        }

        public async Task<ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>>
     GetGameHistoryAsync(ConnectionRequestDTO<GetGameHistoryRequestDTO> requestDTO)
        {
            logger.LogInformation(
                "Getting game history. GameId: {GameId}",
                requestDTO.Data.GameId);

            var gameHistoryResult =
                await chessGameHistoryRepository.GetGameHistoryByGameIdAsync(
                    requestDTO.Data.GameId);

            if (!gameHistoryResult.Any())
            {
                logger.LogWarning(
                    "Game history not found. GameId: {GameId}",
                    requestDTO.Data.GameId);

                return ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>
                    .CreateErrorResponse(
                        null!,
                        ChessGameResponseMessage.InvalidData,
                        HttpStatusCode.BadRequest,
                        []);
            }

            var historiesDTO = new GetGameHistoryResponseDTO
            {
                GameId = requestDTO.Data.GameId
            };

            foreach (var history in gameHistoryResult)
                historiesDTO.FEN.Add(history);

            logger.LogInformation(
                "Game history successfully retrieved. GameId: {GameId}, MovesCount: {Count}",
                requestDTO.Data.GameId,
                historiesDTO.FEN.Count);

            return ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(
                    historiesDTO,
                    ChessGameResponseMessage.SuccessData,
                    HttpStatusCode.OK);
        }

    }
}
