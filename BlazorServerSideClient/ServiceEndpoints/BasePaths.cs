using static ChessGameBlazorClient.ServiceEndpoints.Actions;
using static ChessGameBlazorClient.ServiceEndpoints.Endpoints;

namespace ChessGameBlazorClient.ServiceEndpoints
{
    public class BasePaths
    {
        private IConfiguration _config;
        public BasePaths(IConfiguration config)
        {
            _config = config;
            _baseUrl = _config["ServiceUrls:GatewayApi"];
            _hubUrl = _config["ServiceUrls:ChessGameApi-socket"];
        }
        // Use a property or field that ensures a trailing slash
        private readonly string _baseUrl;

        private readonly string _hubUrl;

        // Removed 'static' so it can access the injected config values
        public string BaseUrlHub => _hubUrl;
        internal string BaseUrl => _baseUrl;

        public Uri GetPath(IdentityEndpoints controller, IdentityAction action) =>
            new Uri($"{_baseUrl}/api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        public Uri GetPath(IdentityEndpoints controller, UserAction action) =>
            new Uri($"{_baseUrl}/api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        public Uri GetPath(ChessGameEndpoints controller, ChessGameAction action) =>
            new Uri($"{_baseUrl}/api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");

        public Uri GetPath(ChatEndpoints controller, ChatAction action) =>
            new Uri($"{_baseUrl}/api/{controller}/{action.ToString().ToLower().Replace("_", "-")}");
    }
}