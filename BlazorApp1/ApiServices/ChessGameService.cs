using WebAssemblyChessGame.UI.ClientService;

namespace WebAssemblyChessGame.UI.ApiServices
{
    public class ChessGameService : BaseHttpClient
    {
        public ChessGameService(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}
