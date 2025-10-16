using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Repositories;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Models;
using System.Collections.Concurrent;

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

        public async Task<int> InitializeBoardAsync(string player1Id, string player2Id)
        {
            var isCreated = await _chessGameRepository.CreateGame(player1Id, player2Id);
            if (!isCreated)
            {
                _logger.LogError("Failed to create a new game between {Player1} and {Player2}", player1Id, player2Id);
                return -1;
            }

            var gameId = await _chessGameRepository.GetGameIdByPlayers(player1Id, player2Id);
            if (gameId == -1)
                _logger.LogError("Failed to retrieve game ID for players {Player1} and {Player2}", player1Id, player2Id);
            else
                _logger.LogInformation("Game successfully created between {Player1} and {Player2}", player1Id, player2Id);
            
            return gameId;
        }

        public Task<bool> SubmitMoveAsync(int gameId, Position currentPosition, Position movePosition, string player)
        {
            throw new NotImplementedException();
        }

    }
}
