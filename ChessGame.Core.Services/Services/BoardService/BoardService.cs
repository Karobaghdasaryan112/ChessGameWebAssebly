using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Repositories;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Figures;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

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

        public async Task<Guid> InitializeBoardAsync(Guid player1Id, Guid player2Id)
        {
            var isCreated = await _chessGameRepository.CreateGame(player1Id, player2Id);
            if (!isCreated)
            {
                _logger.LogError("Failed to create a new game between {Player1} and {Player2}", player1Id, player2Id);
                return Guid.Empty;
            }

            var gameId = await _chessGameRepository.GetGameIdByPlayers(player1Id, player2Id);
            if (gameId == default)
                _logger.LogError("Failed to retrieve game ID for players {Player1} and {Player2}", player1Id,
                    player2Id);
            else
                _logger.LogInformation("Game successfully created between {Player1} and {Player2}", player1Id,
                    player2Id);

            return gameId;
        }

        public async Task<bool> SubmitMoveAsync(Guid gameId, Position currentPosition, Position movePosition,
            Board currentBoardState)
        {
            var fromBlock = currentBoardState.GetBlockByPosition(currentPosition);
            var toBlock = currentBoardState.GetBlockByPosition(movePosition);
            if (fromBlock.Figure == null)
            {
                _logger.LogWarning("No figure found at position {Position} in game {GameId}", currentPosition, gameId);
                //If there is no figure at the from position, throw exception
                //This should never happen, as the frontend should prevent this
                throw new ArgumentException($"If there is no figure at the from-{fromBlock.Position.VerticalOrientation}{fromBlock.Position.HorizontalOrientation} position");
            }

            var toBlockTemp = toBlock.Figure;
            toBlock.Figure = fromBlock.Figure;
            fromBlock.Figure = default;

            _logger.LogInformation("Move submitted in game {GameId} from {FromPosition} to {ToPosition}", gameId,
                currentPosition, movePosition);

            //Check if king is in check after the move
            //If king is in check, return false
            if (await IsKingCheckedAsync(currentBoardState, currentBoardState.Turn))
            {
                _logger.LogWarning("Move from {FromPosition} to {ToPosition} in game {GameId} would leave king in check",
                    currentPosition, movePosition, gameId);

                fromBlock.Figure = toBlock.Figure;
                toBlock.Figure = toBlockTemp;

                _logger.LogInformation("Move revert in game {GameId} from {FromPosition} to {ToPosition}", gameId,
                    movePosition, currentPosition);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        public async Task<Block> CanClick(FigureColors currentColor, Block currentBlock,
            ClickedBlockInformationDTO previusBlockInformationDTO, Board currentBoard)
        {
            if ((int)currentColor != (int)currentBoard.Turn)
                return await Task.FromResult(default(Block))!;

            var currentBlockFromServer = currentBoard.GetBlockByPosition(currentBlock.Position);

            //if the current player is the same color as the figure on the clicked block and previusly clicked block is null
            if (currentBlock.Figure != null &&
                currentBlock.Figure.FigureColor == currentColor)
            {
                _logger.LogInformation("Player with color {Color} clicked on their own figure at position {Position}",
                    currentColor, currentBlock.Position);
                return await Task.FromResult(currentBlockFromServer);
            }

            //if the current player is clicked previusly and now clicked on a movable or cutable position

            if (previusBlockInformationDTO?.ClickedPosition != null &&
                (currentBlockFromServer.EventColor == EventColors.Cut ||
                 currentBlockFromServer.EventColor == EventColors.Move))
            {
                _logger.LogInformation(
                    "Player with color {Color} is attempting to move from {FromPosition} to {ToPosition}", currentColor,
                    previusBlockInformationDTO.ClickedPosition, currentBlock.Position);
                return await Task.FromResult(currentBlockFromServer);
            }

            return await Task.FromResult(default(Block))!;
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
