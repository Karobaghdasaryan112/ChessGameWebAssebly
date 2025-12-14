using ChessGame.Core.Services.MediatR.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessService.API.ChessGameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryWidgetController(IMediator mediator) : ControllerBase
    {
        [HttpGet("History{playerId:guid}")]
        public async Task<IActionResult> GetHistoryWidgetsAsync(Guid playerId)
        {
            var getHistoryResult =
                await mediator.Send<
                    IResponseTypes<
                        GetAllHistoryWidgetsResponseDTO,
                        ChessGameResponseMessage>>
                (
                    new GetAllHistoryWidgetsQuery<
                        GetAllHistoryWidgetRequestDTO,
                        IResponseTypes<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>(
                        new GetAllHistoryWidgetRequestDTO()
                        {
                            CurrentPlayerId = playerId
                        }));
            return
                !getHistoryResult.IsSuccess ? BadRequest(getHistoryResult) : Ok(getHistoryResult);
        }
    }
}
