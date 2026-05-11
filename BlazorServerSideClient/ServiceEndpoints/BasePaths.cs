using static ChessGameBlazorClient.ServiceEndpoints.Actions;
using static ChessGameBlazorClient.ServiceEndpoints.Endpoints;

namespace ChessGameBlazorClient.ServiceEndpoints
{
    public class BasePaths(IConfiguration config)
    {
        // Use a property or field that ensures a trailing slash
        private readonly string _baseUrl = config["ServiceUrls:GatewayApi"]?.TrimEnd('/') + "/" 
                                           ?? "http://gateway.api:8080/";

        private readonly string _hubUrl = config["ServiceUrls:ChessGameApi-socket"] 
                                          ?? "http://chessgame.api:8080/gameHub";

        // Removed 'static' so it can access the injected config values
        public string BaseUrlHub => _hubUrl;
        internal string BaseUrl => _baseUrl;

        public Uri GetPath(IdentityEndpoints controller, IdentityAction action) =>
            new Uri($"{_baseUrl}api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        public Uri GetPath(IdentityEndpoints controller, UserAction action) =>
            new Uri($"{_baseUrl}api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        public Uri GetPath(ChessGameEndpoints controller, ChessGameAction action) =>
            new Uri($"{_baseUrl}api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        public Uri GetPath(ChatEndpoints controller, ChatAction action) =>
            new Uri($"{_baseUrl}api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");
    }
}