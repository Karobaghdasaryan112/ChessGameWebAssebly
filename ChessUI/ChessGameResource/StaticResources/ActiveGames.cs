using SharedResources.ChessGameResource.Models;
using System.Collections.Concurrent;

namespace SharedResources.ChessGameResource.StaticResources
{
    public static class ActiveGames
    {
        private static ConcurrentDictionary<int, Board> _activeGamesAndBoards = new();
        public static Board GetBoard(int gameId) => _activeGamesAndBoards.TryGetValue(gameId, out var board) ? board : null;
        public static ConcurrentDictionary<int, Board> ActiveGamesAndBoards => _activeGamesAndBoards;
        public static bool RemoveGame(int gameId) => _activeGamesAndBoards.TryRemove(gameId, out _);
        public static void ClearAllGames() => _activeGamesAndBoards.Clear();
        public static bool AddGame(int gameId, Board board) => _activeGamesAndBoards.TryAdd(gameId, board);
    }
}
