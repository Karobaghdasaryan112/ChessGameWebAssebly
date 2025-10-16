using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedResources.Requests;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessService.API.ChessGameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChessGameController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ChessGameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Accepts a game by initializing a new chess board for two players.
        /// Delegates the request to the <see cref="BoardInitializeCommand"/>.
        /// </summary>
        /// <param name="request">The request data containing the player IDs to initialize the board.</param>
        /// <returns>
        /// HTTP 200 OK with the result if the board was initialized successfully;
        /// HTTP 400 BadRequest if the operation failed.
        /// </returns>
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptGame([FromBody] BoardInitializeRequestDTO request)
        {
            var command = new BoardInitializeCommand<
                IRequestTypes<BoardInitializeRequestDTO>,
                IResponseTypes<BoardInitializeResponseDTO,ChessGameResponseMessage>>
                (new ChessGameRequest<BoardInitializeRequestDTO>(request));

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Submits a player's move during an ongoing chess game.
        /// Delegates the move processing to the <see cref="SubmitMoveCommand"/>.
        /// </summary>
        /// <param name="request">The request data containing move details (source and destination positions).</param>
        /// <returns>
        /// HTTP 200 OK with the result if the move was successfully processed;
        /// HTTP 400 BadRequest if the move was invalid or failed.
        [HttpPost("move")]
        public async Task<IActionResult> SubmitMove([FromBody] SubmitMoveRequestDTO request)
        {
            var command = new SubmitMoveCommand<
                IRequestTypes<SubmitMoveRequestDTO>,
                IResponseTypes<SubmitMoveResponseDTO, ChessGameResponseMessage>>
                (new ChessGameRequest<SubmitMoveRequestDTO>(request));
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

    }
}
