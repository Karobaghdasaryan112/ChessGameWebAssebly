using ChessGameBlazorClient.Contracts;
using ChessGameBlazorClient.ServiceEndpoints;
using ChessGameBlazorClient.UI.ClientService;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;

namespace ChessGameBlazorClient.ApiServices
{
    public class ChessGameService(HttpClient httpClient, IQueryBuilder queryBuilder)
        : BaseHttpClient(httpClient, queryBuilder)
    {
        public async Task<ChessGameResponse<GetAllHistoryWidgetsResponseDTO>?> GetAllHistoryWidgetsAsync(GetAllHistoryWidgetRequestDTO allHistoryWidgetRequestDto)
        {
            var requestUri = this.BuildRequestUri(
                Endpoints.ChessGameEndpoints.ChessGame,
                Actions.ChessGameAction.History,
                [

                    new KeyValuePair<string, string>("playerId", $"{allHistoryWidgetRequestDto.CurrentPlayerId}")
                ]);

            return await GetAsync<
                ChessGameResponse<GetAllHistoryWidgetsResponseDTO>,
                GetAllHistoryWidgetsResponseDTO,
                ChessGameResponseMessage>(requestUri.ToString());
        }
    }
}
}