using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Requests;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Commands
{
    public class SubmitMoveCommandHandler(
        IMediator mediator,
        IValidator<SubmitMoveRequestDTO> validator,
        ILogger<SubmitMoveCommandHandler> logger,
        IBoardService service)
        :
            MediatR_Base<SubmitMoveRequestDTO, SubmitMoveCommandHandler, IBoardService>(validator, logger, service),
            IRequestHandler<
                SubmitMoveCommand<
                    IRequestTypes<SubmitMoveRequestDTO>,
                    IResponseTypes<SubmitMoveResponseDTO, ChessGameResponseMessage>>,
                IResponseTypes<SubmitMoveResponseDTO, ChessGameResponseMessage>>
    {
        public async Task<IResponseTypes<SubmitMoveResponseDTO, ChessGameResponseMessage>> Handle(
            SubmitMoveCommand<IRequestTypes<SubmitMoveRequestDTO>,
            IResponseTypes<SubmitMoveResponseDTO,
            ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var fromPosition = request.RequestDTO.requestType.From;
            var toPosition = request.RequestDTO.requestType.To;
            var gameId = request.RequestDTO.requestType.GameId;
            var currentBoardState = request.RequestDTO.requestType.CurrentBoardState;


            var response = ChessGameResponse<SubmitMoveResponseDTO>.CreateSuccessResponse(
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

            logger.LogInformation("Move submitted in game {GameId} from {FromPosition} to {ToPosition}", gameId,
                fromPosition, toPosition);

            var requestQuery = new IsKingCheckedRequestDTO()
            {
                ChosenColor = currentBoardState.Turn,
                CurrentBoard = currentBoardState
            };

            var query =
                new IsKingCheckedQuery<IRequestTypes<IsKingCheckedRequestDTO>,
                    IResponseTypes<IsKingCheckedResponseDTO, ChessGameResponseMessage>>(
                    new ChessGameRequest<IsKingCheckedRequestDTO>()
                    {
                        requestType = requestQuery,
                    });

            var isKingCheckedResult = await mediator.Send(query, cancellationToken);

            if (isKingCheckedResult.IsSuccess && !isKingCheckedResult.Data.IsKingChecked)
            {
                return response;
            }
            //Save the From and To move positions in DB



            logger.LogWarning(
            "Move from {FromPosition} to {ToPosition} in game {GameId} would leave king in check",
            fromPosition, toPosition, gameId);

            //Revert the Move
            fromBlock.Figure = toBlock.Figure;
            toBlock.Figure = toBlockTemp;

            response.Data.IsKingChecked = true;


            logger.LogInformation("Move revert in game {GameId} from {FromPosition} to {ToPosition}", gameId,
                fromPosition, toPosition);

            return response;
        }
    }
}
