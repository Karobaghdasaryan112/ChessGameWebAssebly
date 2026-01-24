using ChessGame.Core.Services.MediatR.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessService.API.ChessGameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChessGameController(IMediator mediator) : ControllerBase
    {
        [HttpGet("history")]
        public async Task<IActionResult> GetHistoryWidgetsAsync([FromQuery] Guid playerId)
        {
            var allHistoryWidgetsRequest =
                new GetAllHistoryWidgetRequestDTO()
                {
                    CurrentPlayerId = playerId
                };
            var requestQuery = new GetAllHistoryWidgetsQuery<GetAllHistoryWidgetRequestDTO,
                ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>(allHistoryWidgetsRequest);

            var getHistoryResult = await mediator.Send(requestQuery);

            return
                !getHistoryResult.IsSuccess ? BadRequest(getHistoryResult) : Ok(getHistoryResult);
        }

        [HttpGet("historyPagination")]
        public async Task<IActionResult> GetHistoryWidgetsPaginationByOpponentAsync(
            [FromQuery] Guid currentPlayerId,
            [FromQuery] Guid opponentPlayerId,
            [FromQuery] int currentPage,
            [FromQuery] int pageSize)
        {
            var historyWidgetsPaginationByOpponentRequest =
                new GetGamesByCurrentAndOpponentIdsPaginationRequestDTO()
                {
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    CurrentPlayerGuid = currentPlayerId,
                    OpponentPlayerGuid = opponentPlayerId
                };

            var requestQuery = new GetHistoryWidgetsPaginationQuery<
                GetGamesByCurrentAndOpponentIdsPaginationRequestDTO,
                ResponseDTO<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>>(
                historyWidgetsPaginationByOpponentRequest);

            var gamesPaginationResult = await mediator.Send(requestQuery);

            return
                !gamesPaginationResult.IsSuccess ? BadRequest(gamesPaginationResult) : Ok(gamesPaginationResult);
        }

        [HttpGet("gameHistory")]
        public async Task<IActionResult> GetGameHistoryByGameIdAndPlayerIdAsync([FromQuery] Guid gameId)
        {
            var gameHistoryRequest =
                new GetGameHistoryRequestDTO()
                {
                    GameId = gameId
                };

            var requestQuery = new GetGameHistoryQuery<
                    GetGameHistoryRequestDTO,
                    ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>>
                (gameHistoryRequest);

            var gameHistoryResult = await mediator.Send(requestQuery);

            return
                !gameHistoryResult.IsSuccess ? BadRequest(gameHistoryResult) : Ok(gameHistoryResult);
        }

        [HttpGet("getOptimizedMove")]
        public async Task<IActionResult> GetOptimizedMoveAync([FromQuery] Guid gameId, [FromQuery] FigureColors myColor,
            [FromQuery] bool isMaximizingPlayer)
        {
            var optimizedMoveRequest =
                new GetOptimizedMoveRequestDTO()
                {
                    ChosenColor = myColor,
                    GameId = gameId
                };
            var requestQuery = new GetOptimizedMoveQuery<
                    GetOptimizedMoveRequestDTO,
                    ResponseDTO<GetOptimizedMoveResponseDTO, ChessGameResponseMessage>>
                (optimizedMoveRequest);
            var optimizedMoveResult = await mediator.Send(requestQuery);
            return
                !optimizedMoveResult.IsSuccess ? BadRequest(optimizedMoveResult) : Ok(optimizedMoveResult);
        }
    }
}