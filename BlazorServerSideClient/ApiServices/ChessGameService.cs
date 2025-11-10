using ChessGameBlazorClient.Contracts;
using ChessGameBlazorClient.UI.ClientService;

namespace ChessGameBlazorClient.ApiServices
{
    public class ChessGameService : BaseHttpClient
    {
        public ChessGameService(HttpClient httpClient,IQueryBuilder queryBuilder) : base(httpClient,queryBuilder)
        {

        }
    }
}
