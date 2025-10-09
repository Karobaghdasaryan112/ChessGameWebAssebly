using WebAssemblyChessGame.UI.ClientService;
using WebAssemblyChessGame.UI.Contracts;

namespace WebAssemblyChessGame.UI.ApiServices
{
    public class ChessGameService : BaseHttpClient
    {
        public ChessGameService(HttpClient httpClient,IQueryBuilder queryBuilder) : base(httpClient,queryBuilder)
        {
        }
    }
}
