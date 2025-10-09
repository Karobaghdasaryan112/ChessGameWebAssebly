using WebAssemblyChessGame.UI.ClientService;
using WebAssemblyChessGame.UI.Contracts;

namespace WebAssemblyChessGame.UI.ApiServices
{
    public class NotificationService(HttpClient httpClient, IQueryBuilder queryBuilder) : BaseHttpClient(httpClient, queryBuilder)
    {
    }
}
