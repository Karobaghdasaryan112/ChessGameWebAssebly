using static WebAssemblyChessGame.UI.ServiceEndpoints.Actions;
using static WebAssemblyChessGame.UI.ServiceEndpoints.Endpoints;

namespace WebAssemblyChessGame.UI.ServiceEndpoints
{
    public static class BasePaths
    {
        public static Uri GetPath(IdentityEndpoints controller, IdentityAction action) =>
           new Uri($"api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        public static Uri GetPath(IdentityEndpoints controller, UserAction action) =>
           new Uri($"api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        public static Uri GetPath(ChessGameEndpoints controller, ChessGameAction action) =>
            new Uri($"api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        public static Uri GetPath(ChatEndpoints controller, ChatAction action) =>
            new Uri($"api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");
    }
}
