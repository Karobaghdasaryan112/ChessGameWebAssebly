using ChessGameBlazorClient.Contracts;
using ChessGameBlazorClient.ServiceEndpoints;
using ChessGameBlazorClient.UI.ClientService;

namespace ChessGameBlazorClient.ApiServices
{
    public class NotificationService(HttpClient httpClient, IQueryBuilder queryBuilder,BasePaths basePath) : BaseHttpClient(httpClient, queryBuilder,basePath)
    {
    }
}
