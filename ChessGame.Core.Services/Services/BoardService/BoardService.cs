using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Domain.Domain.Entities;
using Microsoft.Extensions.Logging;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;
using System.Net;
using BoardInitializeRequestDTO = SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs.BoardInitializeRequestDTO;

namespace ChessGame.Core.Services.Services.BoardService
{
    public class BoardService(
        IChessGameUnitOfWork unitOfWork,
        ILogger<BoardService> logger,
        IChessGameRepository chessGameRepository,
        IChessGameHistoryRepository chessGameHistoryRepository)
        : IBoardService
    {
        public async Task<ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>>
            InitializeBoardAsync(BoardInitializeRequestDTO connectionRequestDto)
        {
            try
            {
                chessGameRepository.CreateGame(
                    connectionRequestDto.Player1Id,
                    connectionRequestDto.Player2Id,
             
                    connectionRequestDto.GameEvent,

                    connectionRequestDto.Player1Name,
                    connectionRequestDto.Player2Name,

                    (int)connectionRequestDto.Player1Time.TotalSeconds,
                    (int)connectionRequestDto.Player2Time.TotalSeconds);


                var isCreated = await unitOfWork.SaveChangesAsync(cancellationToken: CancellationToken.None);

                if (!isCreated)
                {
                    logger.LogError("Failed to create a new game between {Player1} and {Player2}",
                        connectionRequestDto.Player1Id, connectionRequestDto.Player2Id);
                    return ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        new BoardInitializeResponseDTO()
                        {
                            GameId = Guid.Empty
                        }, ChessGameResponseMessage.GameCreationFailed, HttpStatusCode.BadRequest,
                        [
                            "Failed to create a new game between {Player1} and {Player2}",
                            connectionRequestDto.Player1Id.ToString(), connectionRequestDto.Player2Id.ToString()
                        ]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           

            var gameId = await chessGameRepository.GetGameIdByPlayers(connectionRequestDto.Player1Id,
                connectionRequestDto.Player2Id);
            if (gameId == Guid.Empty)
            {
                logger.LogError("Failed to retrieve game ID for players {Player1} and {Player2}",
                    connectionRequestDto.Player1Id, connectionRequestDto.Player2Id);
                return ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new BoardInitializeResponseDTO()
                    {
                        GameId = Guid.Empty
                    }, ChessGameResponseMessage.GameCreationFailed, HttpStatusCode.BadRequest,
                    [
                        "Failed to retrieve game ID for players {Player1} and {Player2}",
                        connectionRequestDto.Player1Id.ToString(), connectionRequestDto.Player2Id.ToString()
                    ]);
            }
            else
                logger.LogInformation("Game successfully created between {Player1} and {Player2}",
                    connectionRequestDto.Player1Id, connectionRequestDto.Player2Id);

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
            SaveGameEventAndWinnerRequestDTO connectionRequestDTO)
        {
            logger.LogInformation("SaveGameEventAndWinnerAsync started. Request: {@Request}", connectionRequestDTO);

            if (connectionRequestDTO == null)
            {
                logger.LogWarning("Request or Request.Data is null");

                return ResponseDTO<SaveGameEventAndWinnerResponseDTO, ChessGameResponseMessage>
                .CreateErrorResponse(new SaveGameEventAndWinnerResponseDTO { IsSaved = false }, ChessGameResponseMessage.InvalidData, HttpStatusCode.BadRequest);
            }

            if (connectionRequestDTO.GameId == Guid.Empty || connectionRequestDTO.WinnerPlayerGuid == Guid.Empty)
            {
                logger.LogWarning("Invalid GameId or WinnerPlayerGuid. GameId: {GameId}, Winner: {Winner}", connectionRequestDTO.GameId, connectionRequestDTO.WinnerPlayerGuid);

                return ResponseDTO<SaveGameEventAndWinnerResponseDTO, ChessGameResponseMessage>
                .CreateErrorResponse(new SaveGameEventAndWinnerResponseDTO { IsSaved = false }, ChessGameResponseMessage.InvalidData, HttpStatusCode.BadRequest);
            }

            try
            {
                await chessGameRepository.SaveGameResult(connectionRequestDTO.WinnerPlayerGuid, connectionRequestDTO.GameId);

                var isSaved = await unitOfWork.SaveChangesAsync(cancellationToken: CancellationToken.None);

                if (!isSaved)
                {
                    logger.LogWarning("SaveGameResult returned false. GameId: {GameId}, Winner: {Winner}", connectionRequestDTO.GameId, connectionRequestDTO.WinnerPlayerGuid);

                    return ResponseDTO<SaveGameEventAndWinnerResponseDTO, ChessGameResponseMessage>
                    .CreateErrorResponse(new SaveGameEventAndWinnerResponseDTO { IsSaved = false }, ChessGameResponseMessage.InternalServerError, HttpStatusCode.InternalServerError);
                }

                logger.LogInformation("Game result successfully saved. GameId: {GameId}, Winner: {Winner}", connectionRequestDTO.GameId, connectionRequestDTO.WinnerPlayerGuid);

                return ResponseDTO<SaveGameEventAndWinnerResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(new SaveGameEventAndWinnerResponseDTO { IsSaved = true }, ChessGameResponseMessage.MoveSuccessful, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while saving game result. GameId: {GameId}", connectionRequestDTO?.GameId);

                return ResponseDTO<SaveGameEventAndWinnerResponseDTO, ChessGameResponseMessage>
                .CreateErrorResponse(new SaveGameEventAndWinnerResponseDTO { IsSaved = false }, ChessGameResponseMessage.InternalServerError, HttpStatusCode.InternalServerError);
            }
        }


        public async Task<ResponseDTO<SavePositionsResponseDTO, ChessGameResponseMessage>> SavePositionsAsync(SavePositionsRequestDTO savePositionsRequest)
        {
            var chessGameHistoryModel = new ChessGameHistory()
            {
                FEN = savePositionsRequest.FEN,
                GameId = savePositionsRequest.GameId,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await chessGameHistoryRepository.SaveGameStateAsync(chessGameHistoryModel);
            var isGameStateSaved = await unitOfWork.SaveChangesAsync(cancellationToken: CancellationToken.None);

            return
                isGameStateSaved
                ? ResponseDTO<SavePositionsResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    new SavePositionsResponseDTO()
                    {
                        IsSave = true
                    },
                    ChessGameResponseMessage.GameCreated, HttpStatusCode.OK)
                : ResponseDTO<SavePositionsResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new SavePositionsResponseDTO()
                    {
                        IsSave = false
                    },
                    ChessGameResponseMessage.InternalServerError, HttpStatusCode.InternalServerError, ["Fail to Save Board State into DB"]);
        }

        public async Task<ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>> GetGameHistoryAsync(GetGameHistoryRequestDTO requestDTO)
        {
            logger.LogInformation("Getting game history. GameId: {GameId}", requestDTO.GameId);

            var gameHistoryResult = await chessGameHistoryRepository.GetGameHistoryByGameIdAsync(requestDTO.GameId);

            if (!gameHistoryResult.Any())
            {
                logger.LogWarning("Game history not found. GameId: {GameId}", requestDTO.GameId);

                return ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>
                    .CreateErrorResponse(null!, ChessGameResponseMessage.InvalidData, HttpStatusCode.BadRequest, []);
            }

            var historiesDTO = new GetGameHistoryResponseDTO
            {
                GameId = requestDTO.GameId
            };

            foreach (var history in gameHistoryResult)
                historiesDTO.FEN.Add(history);

            logger.LogInformation("Game history successfully retrieved. GameId: {GameId}, MovesCount: {Count}", requestDTO.GameId, historiesDTO.FEN.Count);

            return ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(historiesDTO, ChessGameResponseMessage.SuccessData, HttpStatusCode.OK);
        }
    }
}
