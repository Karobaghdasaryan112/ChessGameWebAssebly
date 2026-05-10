using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Repositories;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
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
        public async Task<ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>
            GetAllOpponents(GetAllHistoryWidgetRequestDTO getAllHistoryReqeustDTO)
        {
            var validationResult = await genericValidationService.ValidateAsync(getAllHistoryReqeustDTO);
            if (!validationResult.IsValid)
                return ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                    new GetAllHistoryWidgetsResponseDTO { OpponentHistories = new List<OpponentsHistoryDTO>() },
                    ChessGameResponseMessage.InvalidData,
                    HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(error => error.ErrorMessage).ToList());

            var allGamesResult = await chessGameRepository.GetAllGames(getAllHistoryReqeustDTO.CurrentPlayerId);

            return ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(
                new GetAllHistoryWidgetsResponseDTO()
                {
                    OpponentHistories = allGamesResult
                },
                ChessGameResponseMessage.SuccessData,
                HttpStatusCode.OK);
        }

        public async
            Task<ResponseDTO<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>>
            GetGamesByCurrentAndOpponentIdsPagination(
                GetGamesByCurrentAndOpponentIdsPaginationRequestDTO requestDto)
        {
      

            var currentPage = requestDto.CurrentPage;
            var pageSize = requestDto.PageSize;
            var opponentPlayerGuid = requestDto.OpponentPlayerGuid;
            var currentPlayerGuid = requestDto.CurrentPlayerGuid;

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
                player1Name = paginateGame.Player1Name,
                player2Name = paginateGame.Player2Name,

                Opponent =
                    paginateGame.Player1 == opponentPlayerGuid ? paginateGame.Player1Name : paginateGame.Player2Name,
                WinnerPlayerGuid = paginateGame.WinnerPlayer,

            }));

            return ResponseDTO<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>
                .CreateSuccessResponse(
                    new GetGamesByCurrentAndOpponentIdsPaginationResponseDTO() { AllGamesHistories = allGamesDto },
                    ChessGameResponseMessage.SuccessData,
                    HttpStatusCode.OK);
        }
    }
}
