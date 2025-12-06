using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using System.Collections.Concurrent;

namespace SharedResources.ChessGameResource.StaticResources
{
    public static class ActiveGames
    {
        private static ConcurrentDictionary<Guid, Board> _activeGamesAndBoards = new();
        public static Board? GetBoard(Guid gameId) => _activeGamesAndBoards.TryGetValue(gameId, out var board) ? board : null;
        public static ConcurrentDictionary<Guid, Board> ActiveGamesAndBoards => _activeGamesAndBoards;
        public static bool RemoveGame(Guid gameId) => _activeGamesAndBoards.TryRemove(gameId, out _);
        public static void ClearAllGames() => _activeGamesAndBoards.Clear();
        public static bool AddGame(Guid gameId, Board board) => _activeGamesAndBoards.TryAdd(gameId, board);
        public static ConcurrentDictionary<Guid, UserConnectionDTO> _connections = new();
    }
}
