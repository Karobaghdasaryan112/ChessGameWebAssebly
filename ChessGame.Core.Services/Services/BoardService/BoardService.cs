using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Repositories;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Figures;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using System.Net;
using BoardInitializeRequestDTO = SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs.BoardInitializeRequestDTO;
using SubmitMoveRequestDTO = SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs.SubmitMoveRequestDTO;
using SubmitMoveResponseDTO = SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs.SubmitMoveResponseDTO;

namespace ChessGame.Core.Services.Services.BoardService
{
    public class BoardService : IBoardService
    {
        private readonly ILogger<BoardService> _logger;
        private readonly IChessGameRepository _chessGameRepository;
        private readonly IChessGameHistoryRepository _chessGameHistoryRepository;

        public BoardService(
            ILogger<BoardService> logger,
            IChessGameRepository chessGameRepository,
            IChessGameHistoryRepository chessGameHistoryRepository)
        {
            _logger = logger;
            _chessGameRepository = chessGameRepository;
            _chessGameHistoryRepository = chessGameHistoryRepository;
        }


        public async Task<ConnectionResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>> InitializeBoardAsync(ConnectionRequestDTO<BoardInitializeRequestDTO> connectionRequestDto)
        {

            var isCreated = await _chessGameRepository.CreateGame(connectionRequestDto.Data.Player1Id, connectionRequestDto.Data.Player2Id);

            if (!isCreated)
            {
                _logger.LogError("Failed to create a new game between {Player1} and {Player2}", connectionRequestDto.Data.Player1Id, connectionRequestDto.Data.Player2Id);
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

            var gameId = await _chessGameRepository.GetGameIdByPlayers(connectionRequestDto.Data.Player1Id, connectionRequestDto.Data.Player2Id);
            if (gameId == default)
            {
                _logger.LogError("Failed to retrieve game ID for players {Player1} and {Player2}",
                    connectionRequestDto.Data.Player1Id, connectionRequestDto.Data.Player2Id);
                return ConnectionResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(new BoardInitializeResponseDTO()
                {
                    GameId = Guid.Empty
                }, ChessGameResponseMessage.GameCreationFailed, HttpStatusCode.BadRequest,
                    [
                        "Failed to retrieve game ID for players {Player1} and {Player2}",
                        connectionRequestDto.Data.Player1Id.ToString(), connectionRequestDto.Data.Player2Id.ToString()
                    ]);
            }
            else
                _logger.LogInformation("Game successfully created between {Player1} and {Player2}", connectionRequestDto.Data.Player1Id, connectionRequestDto.Data.Player2Id);

            return ConnectionResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new BoardInitializeResponseDTO()
                {
                    GameId = gameId
                },
                ChessGameResponseMessage.GameCreated,
                HttpStatusCode.Created);
        }


        public async Task<ConnectionResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>> SubmitMoveAsync(SubmitMoveRequestDTO submitMoveRequestDto)
        {
            //Data From RequestDto
            var fromPosition = submitMoveRequestDto.From;
            var toPosition = submitMoveRequestDto.To;
            var gameId = submitMoveRequestDto.GameId;
            var currentBoardState = submitMoveRequestDto.CurrentBoardState;

            //Initialize Response DTO
            var connectionResponse = new ConnectionResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>()
            {
                Data = new SubmitMoveResponseDTO()
                {
                    IsKingChecked = false,
                    IsKingMate = false,
                    IsMoveSuccess = true
                },
                IsSuccess = true
            };

            var fromBlock = currentBoardState.GetBlockByPosition(fromPosition!);
            var toBlock = currentBoardState.GetBlockByPosition(toPosition!);

            if (fromBlock.Figure == null)
            {
                _logger.LogWarning("No figure found at position {Position} in game {GameId}", fromPosition, gameId);

                connectionResponse.Data.IsMoveSuccess = false;
                connectionResponse.Errors = new()
                {
                    $"If there is no figure at the from-{fromBlock.Position.VerticalOrientation}{fromBlock.Position.HorizontalOrientation} position"
                };
                connectionResponse.IsSuccess = false;
                return connectionResponse;
            }

            var toBlockTemp = toBlock.Figure;
            toBlock.Figure = fromBlock.Figure;
            fromBlock.Figure = default;

            _logger.LogInformation("Move submitted in game {GameId} from {FromPosition} to {ToPosition}", gameId,
                fromPosition, toPosition);

            //Check if king is in check after the move
            //If king is in check, return false 
            if (await IsKingCheckedAsync(currentBoardState, currentBoardState.Turn))
            {
                //Implement IsKing Mate state
                //TO DO: If there is Mate state then put the IsMateState to True --- connectionResponse.Data.IsKingMate = true;
                _logger.LogWarning(
                    "Move from {FromPosition} to {ToPosition} in game {GameId} would leave king in check",
                    fromPosition, toPosition, gameId);

                fromBlock.Figure = toBlock.Figure;
                toBlock.Figure = toBlockTemp;

                connectionResponse.Data.IsKingChecked = true;

                _logger.LogInformation("Move revert in game {GameId} from {FromPosition} to {ToPosition}", gameId,
                    fromPosition, toPosition);
            }

            return connectionResponse;
        }



        //Response Block
        //Request FigureColors , Block CurrentBlock
        public async Task<ConnectionResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>> CanClick(ConnectionRequestDTO<CanClickRequestDTO> connectionRequestDto)
        {
            //Request Data
            var figureColor = connectionRequestDto.Data.FigureColor;
            var currentBlock = connectionRequestDto.Data.CurrentBlock;
            var previusBlockInformationDTO = connectionRequestDto.Data.ClickedBlockInformationDto;
            var currentBoard = connectionRequestDto.Data.CurrentBoardBoardState!;


            if ((int)figureColor !=
                (int)currentBoard.Turn)
            {
                _logger.LogWarning("It's not the turn of player with color {Color}",
                    figureColor);

                return await Task.FromResult(ConnectionResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new CanClickResponseDTO()
                    {
                        ClickedBlock = null
                    },
                    ChessGameResponseMessage.InvalidMove,
                    HttpStatusCode.BadRequest,
                    ["It's not the turn of the player"]));
            }

            var currentBlockFromServer = currentBoard.GetBlockByPosition(currentBlock.Position);

            //if the current player is the same color as the figure on the clicked block and previusly clicked block is null
            if (currentBlock.Figure != null &&
                currentBlock.Figure.FigureColor == figureColor)
            {
                _logger.LogInformation("Player with color {Color} clicked on their own figure at position {Position}",
                    figureColor,
                    currentBlock.Position);

                return await Task.FromResult(ConnectionResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    new CanClickResponseDTO()
                    {
                        ClickedBlock = currentBlockFromServer
                    },
                    ChessGameResponseMessage.MoveSuccessful, HttpStatusCode.Accepted));
            }

            //if the current player is clicked previusly and now clicked on a movable or cutable position

            if (previusBlockInformationDTO?.ClickedPosition != null &&
                (currentBlockFromServer.EventColor == EventColors.Cut ||
                 currentBlockFromServer.EventColor == EventColors.Move))
            {
                _logger.LogInformation("Player with color {Color} is attempting to move from {FromPosition} to {ToPosition}",
                    figureColor,
                    previusBlockInformationDTO.ClickedPosition,
                    currentBlock.Position);

                return await Task.FromResult(ConnectionResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    new CanClickResponseDTO()
                    {
                        ClickedBlock = currentBlockFromServer
                    },
                    ChessGameResponseMessage.MoveSuccessful, HttpStatusCode.Accepted));
            }

            return await Task.FromResult(ConnectionResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                new CanClickResponseDTO()
                {
                    ClickedBlock = null
                },
                ChessGameResponseMessage.InvalidMove,
                HttpStatusCode.BadRequest,
                ["It's not the turn of the player"]));
        }




        public async Task<bool> IsKingCheckedAsync(Board currentBoard, Turn chosenColor)
        {
            var myColor = (FigureColors)chosenColor;
            var kingBlock = currentBoard.GetBlockByFigureTypeAndColor(FigureType.King, myColor);

            if (await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Rook, myColor, currentBoard,
                    new() { FigureType.Rook, FigureType.Queen }) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Bishop, myColor, currentBoard,
                    new() { FigureType.Queen, FigureType.Bishop }) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Pawn, myColor, currentBoard,
                    new() { FigureType.Pawn, FigureType.Bishop, FigureType.King, FigureType.Queen }) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Queen, myColor, currentBoard,
                    new() { FigureType.Queen }) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Knight, myColor, currentBoard,
                    new() { FigureType.Knight }) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.King, myColor, currentBoard,
                    new() { FigureType.King }))
                return await Task.FromResult(true);

            return await Task.FromResult(false);
        }

        //private methods
        //if king is in check by a specific figure type, we create a clone of the king block and assign the figure type to it
        // then we get the movable and cuttable blocks of that figure type from the king's position
        // if any of those blocks contain an opponent's figure of the specified type, the king is in check
        // we log the information about the check event for debugging purposes
        // this method is generic and can be used for any figure type
        //Generic method to check if king is checked by a specific figure type
        private async Task<bool> IsKingCheckedBy<TFigureType>(Block kingBlock, TFigureType figureType,
            FigureColors myColor, Board currentBoard, List<FigureType> figureTypes) where TFigureType : Enum
        {
            if (!Enum.IsDefined(typeof(TFigureType), figureType))
                return false;

            var kingBlockClone = new Block
            {
                Position = kingBlock.Position,
                Figure = figureType switch
                {
                    FigureType.Rook => new Rook() { FigureColor = myColor },
                    FigureType.Bishop => new Bishop() { FigureColor = myColor },
                    FigureType.King => new King() { FigureColor = myColor },
                    FigureType.Knight => new Knight() { FigureColor = myColor },
                    FigureType.Pawn => new Pawn() { FigureColor = myColor },
                    FigureType.Queen => new Queen() { FigureColor = myColor },
                    _ => throw new ArgumentException()
                }
            };


            var possibleMovableAndCuttable =
                kingBlockClone.Figure.GetMovableAndCutableBlocks(kingBlockClone.Position, currentBoard, kingBlockClone);
            if (possibleMovableAndCuttable.CutableBlock.Count() != 0)
            {

                var figuresForCheck = possibleMovableAndCuttable.CutableBlock.Where(block =>
                    figureTypes.Contains<FigureType>(block.Figure.FigureType));
                if (figuresForCheck.Any())
                {
                    foreach (var figureForCheck in figuresForCheck)
                    {
                        _logger.LogInformation("King of color {Color} is in check by figure at position {Position}",
                            myColor, figureForCheck.Position);
                    }

                    return await Task.FromResult(true);
                }
            }

            return await Task.FromResult(false);
        }
    }
}
