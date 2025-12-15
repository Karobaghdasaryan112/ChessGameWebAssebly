using Azure.Core;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.Requests;
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
            var allHistoryWidgetsRequest = new ChessGameRequest<GetAllHistoryWidgetRequestDTO>()
            {
                requestType =
                    new GetAllHistoryWidgetRequestDTO()
                    {
                        CurrentPlayerId = playerId
                    }
            };
            var requestQuery = new GetAllHistoryWidgetsQuery<
                IRequestTypes<GetAllHistoryWidgetRequestDTO>,
                IResponseTypes<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>(allHistoryWidgetsRequest);

            var getHistoryResult = await mediator.Send(requestQuery);

            return
                !getHistoryResult.IsSuccess ? BadRequest(getHistoryResult) : Ok(getHistoryResult);
        }
    }
}
