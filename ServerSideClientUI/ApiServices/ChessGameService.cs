using ServerSideClientUI.Contracts;
using System.Net.Http;
using WebAssemblyChessGame.UI.ClientService;

namespace ServerSideClientUI.ApiServices
{
    public class ChessGameService : BaseHttpClient
    {
        public ChessGameService(HttpClient httpClient,IQueryBuilder queryBuilder) : base(httpClient,queryBuilder)
        {
        }
    }
}
