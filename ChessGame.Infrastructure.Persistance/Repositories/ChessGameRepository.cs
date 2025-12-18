using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Domain.Domain.Entities;
using ChessGame.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;
using SharedResources.ChessGameResource.Enums.Events;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;

namespace ChessGame.Infrastructure.Persistance.Repositories
{
    public class ChessGameRepository(ChessGameDbContext chessGameDbContext) : IChessGameRepository
    {
        //Widgets
        /// <summary>
        /// Retrieves a list of opponent history records for the specified player, including the total number of games
        /// played against each opponent.
        /// </summary>
        /// <param name="currentPlayerId">The unique identifier of the player whose game history is to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of opponent history data
        /// transfer objects, each representing an opponent and the total number of games played against them. The list
        /// is empty if the player has not played any games.</returns>
        public async Task<List<OpponentsHistoryDTO>> GetAllGames(Guid currentPlayerId)
        {
            return await chessGameDbContext.ChessGames.AsNoTracking()
                .Where(game =>
                    game.Player1 == currentPlayerId ||
                    game.Player2 == currentPlayerId)
                .GroupBy(game =>
                currentPlayerId == game.Player1 ?
                game.Player2 :
                game.Player1).Select(grpGame =>
                new OpponentsHistoryDTO
                {
                    Opponent = grpGame.First().Player1 == currentPlayerId ? grpGame.First().Player2Name : grpGame.First().Player1Name,
                    OpponentGuid = grpGame.First().Player1 == currentPlayerId ? grpGame.First().Player2 : grpGame.First().Player1,
                    TotalCount = grpGame.Count()
                }).ToListAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of games between the specified current player and opponent, ordered by the most
        /// recently updated games first.
        /// </summary>
        /// <remarks>If there are fewer games than the requested page size or if the page index exceeds
        /// the available data, the returned list may contain fewer items or be empty.</remarks>
        /// <param name="currentPlayerGuid">The unique identifier of the current player whose games are to be retrieved.</param>
        /// <param name="opponentPlayerGuid">The unique identifier of the opponent player to filter games against.</param>
        /// <param name="currentPage">The zero-based index of the page to retrieve. Must be greater than or equal to 0.</param>
        /// <param name="pageSize">The number of games to include in each page. Must be greater than 0.</param>
        /// <returns>A list of games involving both the current player and the opponent, limited to the specified page and page
        /// size. The list is ordered by update date in descending order.</returns>
        public async Task<List<Game>> GetGameStatesByCurrentAndOpponentIdsPagination(
            Guid currentPlayerGuid, Guid opponentPlayerGuid, int currentPage, int pageSize)
        {
            var res = await chessGameDbContext.ChessGames.
                 Where(game =>
                     (game.Player1 == currentPlayerGuid &&
                      game.Player2 == opponentPlayerGuid) ||
                     (game.Player1 == opponentPlayerGuid &&
                      game.Player2 == currentPlayerGuid)).
                 OrderByDescending(game => game.UpdateDate).
                 Skip(currentPage * pageSize).
                 Take(pageSize).
                 ToListAsync();
            return res;
        }

        /// <summary>
        /// Creates a new chess game record with the specified players, event, and time controls.
        /// </summary>
        /// <param name="player1">The unique identifier of the first player.</param>
        /// <param name="player2">The unique identifier of the second player.</param>
        /// <param name="gameEvent">The event associated with the game. Specifies the tournament or match context.</param>
        /// <param name="player1Name">The display name of the first player. Cannot be null or empty.</param>
        /// <param name="player2Name">The display name of the second player. Cannot be null or empty.</param>
        /// <param name="player1Time">The initial time, in seconds, allocated to the first player. Must be non-negative.</param>
        /// <param name="player2Time">The initial time, in seconds, allocated to the second player. Must be non-negative.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the game was
        /// created successfully; otherwise, <see langword="false"/>.</returns>

        public async Task<bool> CreateGame(Guid player1, Guid player2, GameEvent gameEvent, string player1Name, string player2Name, int player1Time, int player2Time)
        {
            chessGameDbContext.ChessGames.Add(
                new Game()
                {
                    Player1Name = player1Name,
                    Player2Name = player2Name,

                    Player1Time = player1Time,
                    Player2Time = player2Time,

                    GameEvent = gameEvent,

                    Player1 = player1,
                    Player2 = player2,

                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow

                });

            return (await chessGameDbContext.SaveChangesAsync()) > 0;
        }


        /// <summary>
        /// Retrieves the unique identifier of the most recently created chess game between the specified players.
        /// </summary>
        /// <remarks>If multiple games exist between the same pair of players, the method returns the
        /// identifier of the most recently created game. The order of the player parameters matters; only games where
        /// player1 and player2 match the specified values in the same order are considered.</remarks>
        /// <param name="player1">The unique identifier of the first player.</param>
        /// <param name="player2">The unique identifier of the second player.</param>
        /// <returns>A <see cref="Guid"/> representing the game identifier if a game exists between the specified players;
        /// otherwise, <see cref="Guid.Empty"/>.</returns>
        public async Task<Guid> GetGameIdByPlayers(Guid player1, Guid player2)
        {
            var game = await chessGameDbContext.
                ChessGames.
                OrderByDescending(game =>
                    game.CreateDate)
                .FirstOrDefaultAsync(game =>
                    game.Player1 == player1 && game.Player2 == player2);
            return game?.Id ?? Guid.Empty;

        }

        /// <summary>
        /// Saves the result of a completed chess game by recording the winner and updating the game status.
        /// </summary>
        /// <remarks>If the specified game does not exist, the method returns <see langword="false"/> and
        /// no changes are made.</remarks>
        /// <param name="winnerPlayer">The unique identifier of the player who won the game.</param>
        /// <param name="gameId">The unique identifier of the chess game to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the game
        /// result was saved successfully; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> SaveGameResult(Guid winnerPlayer, Guid gameId)
        {
            var selectedGame = await chessGameDbContext.ChessGames.Where(game => game.Id == gameId).FirstOrDefaultAsync();
            if (selectedGame == null)
                return false;
            selectedGame.GameEvent = GameEvent.Over;
            selectedGame.WinnerPlayer = winnerPlayer;
            selectedGame.UpdateDate = DateTime.UtcNow;
            chessGameDbContext.Update(selectedGame);
            return await chessGameDbContext.SaveChangesAsync() > 0;
        }
    }
}
