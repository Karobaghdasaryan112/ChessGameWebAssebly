using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Domain.Domain.Entities;
using ChessGame.Infrastructure.Persistance.Data;
using Microsoft.EntityFrameworkCore;
using SharedResources.ChessGameResource.Models;

namespace ChessGame.Infrastructure.Persistance.Repositories
{
    public class ChessGameRepository : IChessGameRepository
    {

        private readonly ChessGameDbContext _chessGameDbContext;
        public ChessGameRepository(ChessGameDbContext chessGameDbContext)
        {
            _chessGameDbContext = chessGameDbContext;
        }
        public Task<bool> AcceptDraw(Guid gameId, string player)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateGame(Guid player1, Guid player2)
        {
            _chessGameDbContext.ChessGames.Add(
                new Game()
                {
                    Player1 = player1,
                    Player2 = player2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            return (await _chessGameDbContext.SaveChangesAsync()) > 0;
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
            var game = await _chessGameDbContext.ChessGames.FirstOrDefaultAsync(game => game.Player1 == player1 && game.Player2 == player2);
            return game?.GameId ?? Guid.Empty;

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
