using ChessGameBlazorClient.Contracts;
using ChessGameBlazorClient.UI.ClientService;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;

namespace ChessGameBlazorClient.ApiServices
{
    public class ChessGameService : BaseHttpClient
    {
        public ChessGameService(HttpClient httpClient,IQueryBuilder queryBuilder) : base(httpClient,queryBuilder)
        {

        }
        //public async Task<GetAllHistoryWidgetsResponseDTO>
    }
}
