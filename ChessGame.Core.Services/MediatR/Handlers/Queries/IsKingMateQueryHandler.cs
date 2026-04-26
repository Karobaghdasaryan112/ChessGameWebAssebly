using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Extentions;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
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
        IsKingMateQuery<IsKingMateRequestDTO,
            ResponseDTO<IsKingMateResponseDTO, ChessGameResponseMessage>>,
        ResponseDTO<IsKingMateResponseDTO, ChessGameResponseMessage>>
{
    public async Task<ResponseDTO<IsKingMateResponseDTO, ChessGameResponseMessage>> Handle(
        IsKingMateQuery<IsKingMateRequestDTO, ResponseDTO<IsKingMateResponseDTO, ChessGameResponseMessage>> request,
        CancellationToken cancellationToken)
    {
        var validationResult = await genericValidation.ValidateAsync(request.Request);
        if (!validationResult.IsValid)
            return ResponseDTO<IsKingMateResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                default(IsKingMateResponseDTO)!,
                ChessGameResponseMessage.InvalidData,
                HttpStatusCode.BadRequest, validationResult.Errors.Select(error =>
                    error.ErrorMessage).ToList());

        var currentBoard = request.Request.CurrentBoard;
        var chosenColor = request.Request.ChosenColor;
        var gameId = request.Request.GameId;

        var result = ResponseDTO<IsKingMateResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
            new IsKingMateResponseDTO()
            {
                IsKingMate = true,
            }, ChessGameResponseMessage.MoveSuccessful, HttpStatusCode.OK);

        if (await IsKingMateByAsync<FigureType>(FigureType.King, chosenColor, currentBoard, gameId, mediator) &&
            await IsKingMateByAsync<FigureType>(FigureType.Queen, chosenColor, currentBoard, gameId, mediator) &&
            await IsKingMateByAsync<FigureType>(FigureType.Rook, chosenColor, currentBoard, gameId, mediator) &&
            await IsKingMateByAsync<FigureType>(FigureType.Knight, chosenColor, currentBoard, gameId, mediator) &&
            await IsKingMateByAsync<FigureType>(FigureType.Bishop, chosenColor, currentBoard, gameId, mediator) &&
            await IsKingMateByAsync<FigureType>(FigureType.Pawn, chosenColor, currentBoard, gameId, mediator))
            return result;
        result.Data.IsKingMate = false;
        return result;
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
            return false;

        var figureBlocks =
            currentBoard.GetBlockByFigureTypeAndColor((FigureType)(object)figureType, (FigureColors)myColor);

        if (!figureBlocks.Any())
            return true;

        foreach (var figureBlock in figureBlocks)
        {
            if ((Turn)myColor != currentBoard?.Turn)
                return false;

            var figureMovableAndCuttable = figureBlock.Figure
                .GetMovableAndCuttableBlocks(figureBlock.Position, currentBoard);


            var cuttable = figureMovableAndCuttable.CutableBlock;
            var movable = figureMovableAndCuttable.MovableBlock;

            var executables = cuttable.Concat(movable);

            var enumerableOfExecutable = executables.ToList();

            if (enumerableOfExecutable.Any(executable =>
                    executable.EventColor is not EventColors.Cut and not EventColors.Move and EventColors.Castle))
                return false;

            var submitMoveRequestDTO = new SubmitMoveRequestDTO()
            {
                CurrentBoardState = currentBoard,
                From = figureBlock.Position,
                GameId = gameId
            };

            foreach (var executable in enumerableOfExecutable)
            {
                submitMoveRequestDTO.To = executable.Position;
                var toBlockFigureTemp = ((Block)currentBoard.GetBlockByPosition(executable.Position).Clone()).Figure;

                var submitMoveCommand =
                    new SubmitMoveCommand<SubmitMoveRequestDTO,
                        ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>>(submitMoveRequestDTO);
                
                var mediatRSubmitMoveResponse = await mediator.Send(submitMoveCommand);
                
                if (mediatRSubmitMoveResponse is { Data.IsKingChecked: true })
                    continue;

                currentBoard.ResetEventableBlocks();

                var fromBlock =
                    currentBoard.GetBlockByPosition(figureBlock.Position);

                var toBlock =
                    currentBoard.GetBlockByPosition(submitMoveRequestDTO.To);

                fromBlock.Figure = toBlock.Figure;
                toBlock.Figure = toBlockFigureTemp;

                return false;
            }
        }
        return true;
    }
}