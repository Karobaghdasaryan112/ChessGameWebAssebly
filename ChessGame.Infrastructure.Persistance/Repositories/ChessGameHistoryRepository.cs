using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Domain.Domain.Entities;
using ChessGame.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;

namespace ChessGame.Infrastructure.Persistance.Repositories
{
    public class ChessGameHistoryRepository(ChessGameDbContext chessGameDbContext) : IChessGameHistoryRepository
    {
        private readonly ChessGameDbContext _chessGameDbContext = chessGameDbContext;

        /// <summary>
        /// Retrieves the list of FEN strings representing the move history for a specified chess game.
        /// </summary>
        /// <remarks>The returned FEN strings are ordered as stored in the database and represent each
        /// move's board state. This method does not track changes to the retrieved entities.</remarks>
        /// <param name="gameId">The unique identifier of the chess game for which to retrieve the history.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of FEN strings, each
        /// representing a game state in the specified chess game's history. The list will be empty if no history exists
        /// for the given game ID.</returns>
        public Task<List<string>> GetGameHistoryByGameIdAsync(Guid gameId)
        =>
             _chessGameDbContext.ChessGamesHistory.AsNoTracking()
                .Where(chessGameHistory => chessGameHistory.GameId == gameId)
                .Select(selectedGameHistory => selectedGameHistory.FEN).ToListAsync();


        /// <summary>
        /// Asynchronously saves the specified chess game history to the database.
        /// </summary>
        /// <remarks>The save operation is performed asynchronously and does not immediately commit
        /// changes to the database. To persist changes, ensure that the database context is saved after calling this
        /// method.</remarks>
        /// <param name="chessGameHistory">The chess game history to be persisted. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task SaveGameStateAsync(ChessGameHistory chessGameHistory)
        => await _chessGameDbContext.ChessGamesHistory.AddAsync(chessGameHistory);

    }
}
