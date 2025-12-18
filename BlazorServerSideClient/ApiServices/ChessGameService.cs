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


        public async Task<ChessGameResponse<GetAllHistoryWidgetsResponseDTO>?> GetAllHistoryWidgetsAsync(
            GetAllHistoryWidgetRequestDTO allHistoryWidgetRequestDto)
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



        public async Task<ChessGameResponse<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO>?> GetGamesPaginationPerOpponentAsync(
                GetGamesByCurrentAndOpponentIdsPaginationRequestDTO andOpponentIdsPaginationRequestDto)
        {
            var requestUri = this.BuildRequestUri(Endpoints.ChessGameEndpoints.ChessGame,
                Actions.ChessGameAction.HistoryPagination, [
                    new KeyValuePair<string, string>("currentPlayerId",$"{andOpponentIdsPaginationRequestDto.CurrentPlayerGuid}"),
                    new KeyValuePair<string, string>("opponentPlayerId",$"{andOpponentIdsPaginationRequestDto.OpponentPlayerGuid}"),
                    new KeyValuePair<string, string>("currentPage",$"{andOpponentIdsPaginationRequestDto.CurrentPage}"),
                    new KeyValuePair<string, string>("pageSize",$"{andOpponentIdsPaginationRequestDto.PageSize}"),
                ]);

            return await
                GetAsync<ChessGameResponse<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO>,
                    GetGamesByCurrentAndOpponentIdsPaginationResponseDTO,
                    ChessGameResponseMessage>(
                    requestUri.ToString());
        }

        public async Task<ChessGameResponse<GetGameHistoryResponseDTO>?> GetGameHistoryAsync(GetGameHistoryRequestDTO getGameHistoryRequestDTO)
        {
            var requestUri = this.BuildRequestUri(Endpoints.ChessGameEndpoints.ChessGame, Actions.ChessGameAction.GameHistory, [
                new KeyValuePair<string,string>("gameId",$"{getGameHistoryRequestDTO.GameId}")
                ]);

            return await
                    GetAsync<ChessGameResponse<GetGameHistoryResponseDTO>,
                        GetGameHistoryResponseDTO,
                        ChessGameResponseMessage>(
                        requestUri.ToString());
        }
    }
}
