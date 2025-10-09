using static WebAssemblyChessGame.UI.ServiceEndpoints.Actions;
using static WebAssemblyChessGame.UI.ServiceEndpoints.Endpoints;

namespace WebAssemblyChessGame.UI.ServiceEndpoints
{
    public static class BasePaths
    {
        private static readonly string baseUrl = "http://localhost:5247/";
        public static Uri GetPath(IdentityEndpoints controller, IdentityAction action) =>
           new Uri($"{baseUrl}api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        public static Uri GetPath(IdentityEndpoints controller, UserAction action) =>
           new Uri($"{baseUrl}api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        public static Uri GetPath(ChessGameEndpoints controller, ChessGameAction action) =>
            new Uri($"{baseUrl}api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        public static Uri GetPath(ChatEndpoints controller, ChatAction action) =>
            new Uri($"{baseUrl}api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");
    }
}
