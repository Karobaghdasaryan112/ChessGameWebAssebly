using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Domain.Domain.Entities;
using ChessGame.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;
using SharedResources.ChessGameResource.Models;

namespace ChessGame.Infrastructure.Persistance.Repositories
{
    public class ChessGameRepository(ChessGameDbContext chessGameDbContext) : IChessGameRepository
    {
        //Widgets
        public async Task<List<Game>> GetAllGames(Guid currentPlayerId)
        {
            return await chessGameDbContext.ChessGames.AsNoTracking()
                .Where(game =>
                    game.Player1 == currentPlayerId ||
                    game.Player2 == currentPlayerId)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllOpponentsPerCurrentPlayerAsync(Guid currentPlayerIdGuid)
        {

            return await chessGameDbContext.
                ChessGames.
                AsNoTracking().
                Where(game =>
                    game.Player1 == currentPlayerIdGuid ||
                    game.Player2 == currentPlayerIdGuid).
                Select(myGame =>
                    myGame.Player1 == currentPlayerIdGuid ? myGame.Player2Name : myGame.Player1Name).
                ToListAsync();
        }

        public async Task<List<Game>> GetGameStatesByCurrentAndOpponentIdsPagination(
            Guid currentPlayerGuid, Guid opponentPlayerGuid, int currentPage, int pageSize)
        {
            return await chessGameDbContext.ChessGames.
                Where(game =>
                    (game.Player1 == currentPlayerGuid &&
                     game.Player2 == opponentPlayerGuid) ||
                    (game.Player1 == opponentPlayerGuid &&
                     game.Player2 == currentPlayerGuid)).
                Skip(currentPage * pageSize).
                Take(pageSize).
                ToListAsync();
        }

        //Widgets

        public Task<bool> AcceptDraw(Guid gameId, string player)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateGame(Guid player1, Guid player2, string player1Name, string player2Name, int player1Time, int player2Time)
        {
            chessGameDbContext.ChessGames.Add(
                new Game()
                {
                    Player1Name = player1Name,
                    Player2Name = player2Name,

                    Player1Time = player1Time,
                    Player2Time = player2Time,

                    Player1 = player1,
                    Player2 = player2,

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow

                });
            return (await chessGameDbContext.SaveChangesAsync()) > 0;
        }

        public Task<bool> DeclineDraw(Guid gameId, string player)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetCurrentTurn(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> GetGameIdByPlayers(Guid player1, Guid player2)
        {
            var game = await chessGameDbContext.ChessGames.FirstOrDefaultAsync(game => game.Player1 == player1 && game.Player2 == player2);
            return game?.Id ?? Guid.Empty;

        }

        public Task<string> GetGameState(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetWinner(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsGameOver(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsPlayerInGame(Guid gameId, string player)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MovePiece(Guid gameId, string player, Block block)
        {
            throw new NotImplementedException();
        }

        public Task<bool> OfferDraw(Guid gameId, string player)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResignGame(Guid gameId, string player)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SubmitMove(Guid gameId, Position currentPosition, Position movePosition, string player)
        {
            throw new NotImplementedException();
        }

    }
}
