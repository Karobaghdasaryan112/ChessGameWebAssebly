using ChessGameBlazorClient.Contracts;
using ChessGameBlazorClient.ServiceEndpoints;
using ChessGameBlazorClient.UI.ClientService;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGameBlazorClient.ApiServices
{
    public class ChessGameService(HttpClient httpClient, IQueryBuilder queryBuilder)
        : BaseHttpClient(httpClient, queryBuilder)
    {


        public async Task<ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>?> GetAllHistoryWidgetsAsync(
            GetAllHistoryWidgetRequestDTO allHistoryWidgetRequestDto)
        {
            var requestUri = this.BuildRequestUri(
                Endpoints.ChessGameEndpoints.ChessGame,
                Actions.ChessGameAction.History,
                [
                    new KeyValuePair<string, string>("playerId", $"{allHistoryWidgetRequestDto.CurrentPlayerId}")
                ]);

            return await GetAsync<
                ResponseDTO<GetAllHistoryWidgetsResponseDTO,ChessGameResponseMessage>,
                GetAllHistoryWidgetsResponseDTO,
                ChessGameResponseMessage>(requestUri.ToString());
        }



        public async Task<ResponseDTO<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>?> GetGamesPaginationPerOpponentAsync(
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
                GetAsync<ResponseDTO<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>,
                    GetGamesByCurrentAndOpponentIdsPaginationResponseDTO,
                    ChessGameResponseMessage>(
                    requestUri.ToString());
        }

        public async Task<ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>?> GetGameHistoryAsync(GetGameHistoryRequestDTO getGameHistoryRequestDTO)
        {
            var requestUri = this.BuildRequestUri(Endpoints.ChessGameEndpoints.ChessGame, Actions.ChessGameAction.GameHistory, [
                new KeyValuePair<string,string>("gameId",$"{getGameHistoryRequestDTO.GameId}")
                ]);

            return await
                    GetAsync<ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>,
                        GetGameHistoryResponseDTO,
                        ChessGameResponseMessage>(
                        requestUri.ToString());
        }
    }
}
