using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Core.Services.Services.Validations;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using System.Net;
using IHistoryWidgetService = ChessGame.Core.Services.Contracts.BoardServices.IHistoryWidgetService;

namespace ChessGame.Core.Services.Services.HubServices
{
    public class HistoryWidgetService(
        IBoardService boardService,
        GenericValidationService genericValidationService,
        IChessGameHistoryRepository chessGameHistoryRepository,
        IChessGameRepository chessGameRepository)
        : IHistoryWidgetService
    {
        public async Task<ConnectionResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>
            GetAllOpponents(ConnectionRequestDTO<GetAllHistoryWidgetRequestDTO> getAllHistoryReqeustDTO)
        {
            var validationResult = await genericValidationService.ValidateAsync(getAllHistoryReqeustDTO.Data);
            if (!validationResult.IsValid)
                return await validationResult.ReturnValidationResult(default(GetAllHistoryWidgetsResponseDTO));

            var allGamesResult = await chessGameRepository.GetAllGames(getAllHistoryReqeustDTO.Data.CurrentPlayerId);

            var allOpponentsResult = new List<HistoryGameDTO>();

            allGamesResult.ForEach(game => allOpponentsResult.Add(new HistoryGameDTO()
            {
                Opponent =
                    game.Player1 == getAllHistoryReqeustDTO.Data.CurrentPlayerId ?
                        game.Player1Name :
                        game.Player2Name
            }));

            return ConnectionResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(new GetAllHistoryWidgetsResponseDTO()
                {
                    AllGamesHistories = allOpponentsResult
                }, ChessGameResponseMessage.SuccessData, HttpStatusCode.OK);
        }

        public async
            Task<IResponseTypes<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>>
            GetGamesByCurrentAndOpponentIdsPagination(
                IRequestTypes<GetGamesByCurrentAndOpponentIdsPaginationRequestDTO> RequestDto)
        {
            var currentPage = RequestDto.requestType.CurrentPage;
            var pageSize = RequestDto.requestType.PageSize;
            var opponentPlayerGuid = RequestDto.requestType.OpponentPlayerGuid;
            var currentPlayerGuid = RequestDto.requestType.CurrentPlayerGuid;

            var gamesPaginationResult =
                await chessGameRepository.
                GetGameStatesByCurrentAndOpponentIdsPagination(
                currentPlayerGuid,
                opponentPlayerGuid,
                currentPage,
                pageSize);

            var allGamesDto = new List<HistoryGameDTO>();

            gamesPaginationResult.ForEach(paginateGame => allGamesDto.Add(new HistoryGameDTO()
            {
                Date = paginateGame.UpdatedAt,
                Duration = TimeSpan.FromMinutes((paginateGame.Player1Time + paginateGame.Player2Time)),
                GameEvent = paginateGame.GameEvent,
                Opponent =
                    paginateGame.Player1 == opponentPlayerGuid ? paginateGame.Player1Name : paginateGame.Player2Name
            }));

            return ChessGameResponse<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO>
                .CreateSuccessResponse(
                    new GetGamesByCurrentAndOpponentIdsPaginationResponseDTO() { AllGamesHistories = allGamesDto },
                    ChessGameResponseMessage.SuccessData,
                    HttpStatusCode.OK,null);
        }
    }
}
