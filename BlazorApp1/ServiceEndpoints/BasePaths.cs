using static WebAssemblyChessGame.UI.ServiceEndpoints.Actions;
using static WebAssemblyChessGame.UI.ServiceEndpoints.Endpoints;

namespace WebAssemblyChessGame.UI.ServiceEndpoints
{
    public static class BasePaths
    {
        internal static readonly string baseUrl = "http://localhost:5247/";
        internal static readonly string baseUrlHub = "http://localhost:5247/gamehub";

        /// <summary>
        /// Builds the URI for the specified identity controller and identity action.
        /// </summary>
        /// <param name="controller">The identity controller (e.g., Identity, Auth).</param>
        /// <param name="action">The action to perform (e.g., Login, Register).</param>
        /// <returns>A complete URI for the API endpoint.</returns>
        public static Uri GetPath(IdentityEndpoints controller, IdentityAction action) =>
           new Uri($"{baseUrl}api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        /// <summary>
        /// Builds the URI for the specified identity controller and user-related action.
        /// </summary>
        /// <param name="controller">The identity controller (e.g., Identity, User).</param>
        /// <param name="action">The user action to perform (e.g., GetProfile, UpdateUser).</param>
        /// <returns>A complete URI for the API endpoint.</returns>
        public static Uri GetPath(IdentityEndpoints controller, UserAction action) =>
           new Uri($"{baseUrl}api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        /// <summary>
        /// Builds the URI for the specified chess game controller and game-related action.
        /// </summary>
        /// <param name="controller">The chess game controller (e.g., Game, Matchmaking).</param>
        /// <param name="action">The game action to perform (e.g., Start, Move).</param>
        /// <returns>A complete URI for the API endpoint.</returns>
        public static Uri GetPath(ChessGameEndpoints controller, ChessGameAction action) =>
            new Uri($"{baseUrl}api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        /// <summary>
        /// Builds the URI for the specified chat controller and chat-related action.
        /// </summary>
        /// <param name="controller">The chat controller (e.g., Chat, Message).</param>
        /// <param name="action">The chat action to perform (e.g., SendMessage, GetMessages).</param>
        /// <returns>A complete URI for the API endpoint.</returns>
        public static Uri GetPath(ChatEndpoints controller, ChatAction action) =>
            new Uri($"{baseUrl}api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");
    }
}
