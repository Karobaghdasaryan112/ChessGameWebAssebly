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

            return ConnectionResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(
                new GetAllHistoryWidgetsResponseDTO()
                {
                    OpponentHistories = allGamesResult
                },
                ChessGameResponseMessage.SuccessData,
                HttpStatusCode.OK);
        }

        public async
            Task<IResponseTypes<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>>
            GetGamesByCurrentAndOpponentIdsPagination(
                IRequestTypes<GetGamesByCurrentAndOpponentIdsPaginationRequestDTO> requestDto)
        {
            var validationResult = await genericValidationService.ValidateAsync(requestDto.requestType);
            if (!validationResult.IsValid)
                return ChessGameResponse<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO>.CreateErrorResponse(
                    ChessGameResponseMessage.InvalidData,
                    HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(error => error.ErrorMessage).ToList());

            var currentPage = requestDto.requestType.CurrentPage;
            var pageSize = requestDto.requestType.PageSize;
            var opponentPlayerGuid = requestDto.requestType.OpponentPlayerGuid;
            var currentPlayerGuid = requestDto.requestType.CurrentPlayerGuid;

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
                GameId = paginateGame.Id,
                Date = paginateGame.UpdateDate,
                Duration = TimeSpan.FromMinutes((paginateGame.Player1Time + paginateGame.Player2Time)),
                GameEvent = paginateGame.GameEvent,
                Opponent =
                    paginateGame.Player1 == opponentPlayerGuid ? paginateGame.Player1Name : paginateGame.Player2Name,
                WinnerPlayerGuid = paginateGame.WinnerPlayer,

            }));

            return ChessGameResponse<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO>
                .CreateSuccessResponse(
                    new GetGamesByCurrentAndOpponentIdsPaginationResponseDTO() { AllGamesHistories = allGamesDto },
                    ChessGameResponseMessage.SuccessData,
                    HttpStatusCode.OK, null);
        }
    }
}
