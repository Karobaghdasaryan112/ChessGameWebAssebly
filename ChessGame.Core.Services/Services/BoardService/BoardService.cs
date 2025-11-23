using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Repositories;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
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
                _logger.LogError("Failed to retrieve game ID for players {Player1} and {Player2}", player1Id, player2Id);
            else
                _logger.LogInformation("Game successfully created between {Player1} and {Player2}", player1Id, player2Id);

            return gameId;
        }

        public Task<bool> SubmitMoveAsync(Guid gameId, Position currentPosition, Position movePosition, Board currentBoardState)
        {
            var fromBlock = currentBoardState.GetBlockByPosition(currentPosition);
            var toBlock = currentBoardState.GetBlockByPosition(movePosition);
            if (fromBlock.Figure == null)
            {
                _logger.LogWarning("No figure found at position {Position} in game {GameId}", currentPosition, gameId);
                return Task.FromResult(false);
            }
            toBlock.Figure = fromBlock.Figure;
            fromBlock.Figure = null;
            _logger.LogInformation("Move submitted in game {GameId} from {FromPosition} to {ToPosition}", gameId, currentPosition, movePosition);
            return Task.FromResult(true);
        }

        public Task<bool> CanClick(FigureColors currentColor, Block currentBlock, ClickedBlockInformationDTO previusBlockInformationDTO)
        {

            //if the current player is the same color as the figure on the clicked block and previusly clicked block is null
            if (currentBlock.Figure != null &&
                currentBlock.Figure.FigureColor == currentColor)
            {
                _logger.LogInformation("Player with color {Color} clicked on their own figure at position {Position}", currentColor, currentBlock.Position);
                return Task.FromResult(true);
            }

            //if the current player is clicked previusly and now clicked on a movable or cutable position
            if (previusBlockInformationDTO?.ClieckedBlock != null &&
                (currentBlock.EventColor == EventColors.Cut || currentBlock.EventColor == EventColors.Move))
            {
                _logger.LogInformation("Player with color {Color} is attempting to move from {FromPosition} to {ToPosition}", currentColor, previusBlockInformationDTO.ClieckedBlock.Position, currentBlock.Position);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

    }
}
