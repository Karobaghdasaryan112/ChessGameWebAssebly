using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Extentions;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Core.Services.Services.Validations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Requests;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Queries;

/// <summary>
/// MediatR query handler responsible for determining whether the king of the specified color is in checkmate.
/// It simulates every legal move of the player's pieces and checks if any move results in a position where the king is no longer in check.
/// If all simulated moves still leave the king in check, the position is checkmate.
/// </summary>
public class IsKingMateQueryHandler(
    IValidator<IsKingMateRequestDTO> validator,
    ILogger<IsKingMateQueryHandler> logger,
    IBoardService boardService,
    IMediator mediator,
    GenericValidationService genericValidation) :
    MediatR_Base<IsKingMateRequestDTO, IsKingMateQueryHandler, IBoardService>(validator, logger,
        boardService),
    IRequestHandler<
        IsKingMateQuery<IRequestTypes<IsKingMateRequestDTO>,
            IResponseTypes<IsKingMateResponseDTO, ChessGameResponseMessage>>,
        IResponseTypes<IsKingMateResponseDTO, ChessGameResponseMessage>>
{
    public async Task<IResponseTypes<IsKingMateResponseDTO, ChessGameResponseMessage>>
        Handle(
            IsKingMateQuery<IRequestTypes<IsKingMateRequestDTO>,
                IResponseTypes<IsKingMateResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
    {

        var validationResult = await genericValidation.ValidateAsync(request.Request.requestType);
        if (!validationResult.IsValid)
            return ChessGameResponse<IsKingMateResponseDTO>.CreateErrorResponse(
                default(IsKingMateResponseDTO),
                ChessGameResponseMessage.InvalidData,
                HttpStatusCode.BadRequest, validationResult.Errors.Select(error =>
                    error.ErrorMessage).ToList());

        var currentBoard = request.Request.requestType.CurrentBoard;
        var chosenColor = request.Request.requestType.ChosenColor;
        var gameId = request.Request.requestType.GameId;
        var result = ChessGameResponse<IsKingMateResponseDTO>.CreateSuccessResponse(new IsKingMateResponseDTO()
        {
            IsKingMate = true,
        }, ChessGameResponseMessage.MoveSuccessful, HttpStatusCode.OK, null);
        if (await IsKingMateByAsync<FigureType>(FigureType.King, chosenColor, currentBoard, gameId, mediator) &&
            await IsKingMateByAsync<FigureType>(FigureType.Queen, chosenColor, currentBoard, gameId, mediator) &&
            await IsKingMateByAsync<FigureType>(FigureType.Rook, chosenColor, currentBoard, gameId, mediator) &&
            await IsKingMateByAsync<FigureType>(FigureType.Knight, chosenColor, currentBoard, gameId, mediator) &&
            await IsKingMateByAsync<FigureType>(FigureType.Bishop, chosenColor, currentBoard, gameId, mediator) &&
            await IsKingMateByAsync<FigureType>(FigureType.Pawn, chosenColor, currentBoard, gameId, mediator))
            return await Task.FromResult(result);
        result.Data.IsKingMate = false;
        return await Task.FromResult(result);
    }

    /// <summary>
    /// Checks if all pieces of the given figure type have no legal move that escapes check.
    /// Returns true if this piece type cannot help escape check (i.e., all its moves either don't exist or still leave king in check).
    /// Used in conjunction with other piece types to determine full checkmate.
    /// </summary>
    private async Task<bool> IsKingMateByAsync<TFigureType>(TFigureType figureType, Turn myColor,
        Board? currentBoard, Guid gameId, IMediator mediator) where TFigureType : Enum
    {
        if (currentBoard == null)
            return await Task.FromResult(false);
        var figureBlocks =
            currentBoard.GetBlockByFigureTypeAndColor((FigureType)(object)figureType, (FigureColors)myColor);
        if (!figureBlocks.Any())
            return await Task.FromResult(true);
        foreach (var figureBlock in figureBlocks)
        {
            if ((Turn)myColor != currentBoard?.Turn)
                return await Task.FromResult(false);

            var figureMovableAndCuttable = figureBlock.Figure
                .GetMovableAndCuttableBlocks(figureBlock.Position, currentBoard);
            if (figureMovableAndCuttable is
                    not { MovableBlock: not null, CutableBlock: not null } ||
                (!figureMovableAndCuttable.MovableBlock.Any() &&
                 !figureMovableAndCuttable.CutableBlock.Any()))
                return await Task.FromResult(true);

            var cuttable = figureMovableAndCuttable.CutableBlock;
            var movable = figureMovableAndCuttable.MovableBlock;

            var executables = cuttable.Concat(movable);

            var enumerableOfExecutable = executables.ToList();

            if (enumerableOfExecutable.Any(executable =>
                    executable.EventColor is not EventColors.Cut and not EventColors.Move))
                return await Task.FromResult(false);


            var submitMoveRequestDTO = new SubmitMoveRequestDTO()
            {
                CurrentBoardState = currentBoard,
                From = figureBlock.Position,
                GameId = gameId
            };

            foreach (var executable in enumerableOfExecutable)
            {
                submitMoveRequestDTO.To = executable.Position;
                var toBlockFigureTemp = currentBoard.GetBlockByPosition(executable.Position).Figure;


                var submitMoveRequest = new ChessGameRequest<SubmitMoveRequestDTO>()
                {
                    requestType = submitMoveRequestDTO
                };
                var submitMoveCommand =
                    new SubmitMoveCommand<IRequestTypes<SubmitMoveRequestDTO>,
                        IResponseTypes<SubmitMoveResponseDTO, ChessGameResponseMessage>>(submitMoveRequest);

                var mediatRSubmitMoveResponse = await mediator.Send(submitMoveCommand);


                if (mediatRSubmitMoveResponse is { Data.IsKingChecked: true })
                    continue;

                currentBoard.ResetEventableBlocks();

                var fromBlock =
                    currentBoard.GetBlockByPosition(figureBlock.Position);

                var toBlock =
                    currentBoard.GetBlockByPosition(executable.Position);
                var fromTempFigure = fromBlock.Figure;

                fromBlock.Figure = toBlock.Figure;
                toBlock.Figure = toBlockFigureTemp;

                return false;
            }

            return true;
        }

        return false;
    }
}