using ChessGameBlazorClient.Contracts;
using ChessGameBlazorClient.UI.ClientService;

namespace ChessGameBlazorClient.ApiServices
{
    public class NotificationService(HttpClient httpClient, IQueryBuilder queryBuilder) : BaseHttpClient(httpClient, queryBuilder)
    {
    }
}
