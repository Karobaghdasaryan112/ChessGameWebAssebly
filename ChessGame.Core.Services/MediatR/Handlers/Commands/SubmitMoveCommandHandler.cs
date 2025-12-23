using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Extentions;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Commands
{

    /// <summary>
    /// Handles the submission of a chess move command, validating the request, updating the board state, and
    /// determining the result of the move.
    /// </summary>
    /// <remarks>This handler coordinates move validation, board updates, and king safety checks as part of
    /// processing a move submission. It integrates with the application's mediator and logging infrastructure. Thread
    /// safety and error handling are managed according to the application's MediatR and service patterns.</remarks>
    /// <param name="mediator">The mediator used to send queries and commands within the application.</param>
    /// <param name="validator">The validator used to ensure the move request data is valid before processing.</param>
    /// <param name="logger">The logger used to record informational and warning messages during command handling.</param>
    /// <param name="service">The board service that provides operations related to the chess game board.</param>
    public class SubmitMoveCommandHandler(
        IMediator mediator,
        IValidator<SubmitMoveRequestDTO> validator,
        ILogger<SubmitMoveCommandHandler> logger,
        IBoardService service)
        :
            MediatR_Base<SubmitMoveRequestDTO, SubmitMoveCommandHandler, IBoardService>(validator, logger, service),
            IRequestHandler<
                SubmitMoveCommand<
                    SubmitMoveRequestDTO,
                    ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>>,
                ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>>
    {
        /// <summary>
        /// Processes a chess move submission request and determines whether the move is valid, applies the move, and
        /// checks if the king is in check as a result.
        /// </summary>
        /// <remarks>If the move would leave the king in check, the move is reverted and the response
        /// indicates that the king is checked. If there is no figure at the source position, the move is not applied
        /// and the response indicates failure.</remarks>
        /// <param name="request">The command containing the move submission details, including the source and destination positions, game
        /// identifier, and the current board state.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response object with the
        /// outcome of the move submission, including whether the move was successful and if the king is in check.</returns>
        public async Task<ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>> Handle(
            SubmitMoveCommand<SubmitMoveRequestDTO,
            ResponseDTO<SubmitMoveResponseDTO,
            ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var fromPosition = request.RequestDTO.From;
            var toPosition = request.RequestDTO.To;
            var gameId = request.RequestDTO.GameId;
            var currentBoardState = request.RequestDTO.CurrentBoardState;


            var response = ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new SubmitMoveResponseDTO()
                {
                    IsKingChecked = false,
                    IsKingMate = false,
                    IsMoveSuccess = true
                }, ChessGameResponseMessage.MoveSuccessful,
                HttpStatusCode.OK,
                null!);


            var fromBlock = currentBoardState.GetBlockByPosition(fromPosition!);
            var toBlock = currentBoardState.GetBlockByPosition(toPosition!);

            request.RequestDTO.CurrentBoardState.ResetEventableBlocks();

            if (fromBlock?.Figure == null)
            {
                logger.LogWarning("No figure found at position {Position} in game {GameId}", fromPosition, gameId);

                response.Data.IsMoveSuccess = false;
                response.Errors =
                [
                    $"If there is no figure at the from-{fromBlock.Position.VerticalOrientation}{fromBlock.Position.HorizontalOrientation} position"
                ];
                response.IsSuccess = false;
                return response;
            }

            var toBlockTemp = toBlock.Figure;

            toBlock.Figure = fromBlock.Figure;
            fromBlock.Figure = null!;

            logger.LogInformation("Move submitted in game {GameId} from {FromPosition} to {ToPosition}", gameId,fromPosition, toPosition);

            var requestQuery = new IsKingCheckedRequestDTO()
            {
                ChosenColor = currentBoardState.Turn,
                CurrentBoard = currentBoardState
            };

            var query =
                new IsKingCheckedQuery<IsKingCheckedRequestDTO, ResponseDTO<IsKingCheckedResponseDTO, ChessGameResponseMessage>>(requestQuery);

            var isKingCheckedResult = await mediator.Send(query, cancellationToken);

            if (isKingCheckedResult.IsSuccess && !isKingCheckedResult.Data.IsKingChecked)
            {
                return response;
            }
            logger.LogWarning("Move from {FromPosition} to {ToPosition} in game {GameId} would leave king in check",fromPosition, toPosition, gameId);

            fromBlock.Figure = toBlock.Figure;
            toBlock.Figure = toBlockTemp;

            response.Data.IsKingChecked = true;


            logger.LogInformation("Move revert in game {GameId} from {FromPosition} to {ToPosition}", gameId,fromPosition, toPosition);

            return response;
        }
    }
}
