using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Infrastructure.Persistance.Data;
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
        public Task<bool> AcceptDraw(int gameId, string player)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateGame(string player1, string player2)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeclineDraw(int gameId, string player)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetCurrentTurn(int gameId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetGameIdByPlayers(string player1, string player2)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetGameState(int gameId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetWinner(int gameId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsGameOver(int gameId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsPlayerInGame(int gameId, string player)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MovePiece(int gameId, string player, Block block)
        {
            throw new NotImplementedException();
        }

        public Task<bool> OfferDraw(int gameId, string player)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResignGame(int gameId, string player)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SubmitMove(int gameId, Position currentPosition, Position movePosition, string player)
        {
            throw new NotImplementedException();
        }

    }
}
