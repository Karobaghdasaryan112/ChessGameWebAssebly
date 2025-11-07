using ServerSideClientUI.Contracts;
using System.Net.Http;
using WebAssemblyChessGame.UI.ClientService;

namespace ServerSideClientUI.ApiServices
{
    public class NotificationService(HttpClient httpClient, IQueryBuilder queryBuilder) : BaseHttpClient(httpClient, queryBuilder)
    {
    }
}
