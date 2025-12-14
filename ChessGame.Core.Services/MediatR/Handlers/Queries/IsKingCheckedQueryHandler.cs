using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Figures;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Queries
{
    /// <summary>
    /// MediatR query handler that determines whether the king of the chosen color is currently in check
    /// on the provided board state.
    /// 
    /// The handler uses the well-known "king-as-attacker" technique:
    ///   • Temporarily pretends the king is a Queen/Rook/Bishop/Knight (etc.)
    ///   • Calls GetMovableAndCuttableBlocks from that piece's perspective
    ///   • If any enemy piece of the corresponding type(s) lies on those attack lines,
    ///     the real piece is checking the king.
    /// 
    /// This approach correctly detects checks from sliding pieces (R, B, Q) and knights.
    /// King-adjacent checks are also detected with the same method.
    /// 
    /// NOTE: The current implementation contains two critical bugs that must be fixed:
    ///   1. The final line unconditionally overwrites the result with false.
    ///   2. Pawn checks are incorrectly handled with the generic method – pawns require a dedicated
    ///      direction-aware check because their attack is asymmetric.
    /// 
    /// After fixing those issues (and removing unnecessary async/await), this becomes a clean,
    /// performant and maintainable check-detection routine.
    /// </summary>
    public class IsKingCheckedQueryHandler(
        IValidator<IsKingCheckedRequestDTO> validator,
        ILogger<IsKingCheckedQueryHandler> logger,
        IBoardService service)
        : MediatR_Base<IsKingCheckedRequestDTO, IsKingCheckedQueryHandler, IBoardService>
            (validator, logger, service),
            IRequestHandler<
                IsKingCheckedQuery<
                    IRequestTypes<IsKingCheckedRequestDTO>,
                    IResponseTypes<IsKingCheckedResponseDTO, ChessGameResponseMessage>>,
                IResponseTypes<IsKingCheckedResponseDTO, ChessGameResponseMessage>>
    {
        public async Task<IResponseTypes<IsKingCheckedResponseDTO, ChessGameResponseMessage>>
            Handle(
                IsKingCheckedQuery<
                    IRequestTypes<IsKingCheckedRequestDTO>,
                    IResponseTypes<IsKingCheckedResponseDTO,
                        ChessGameResponseMessage>> request,
                CancellationToken cancellationToken)
        {
            var chosenColor = request.Request.requestType.ChosenColor;
            var currentBoard = request.Request.requestType.CurrentBoard;
            var response = ChessGameResponse<IsKingCheckedResponseDTO>.CreateSuccessResponse(
                new IsKingCheckedResponseDTO()
                {
                    IsKingChecked = false
                },
                ChessGameResponseMessage.MoveSuccessful,
                HttpStatusCode.OK,
                null);
            var myColor = (FigureColors)chosenColor;
            var kingBlock = currentBoard.GetBlockByFigureTypeAndColor(FigureType.King, myColor).First();
            if (await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Queen, myColor, currentBoard,
                    [FigureType.Queen]) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Rook, myColor, currentBoard,
                    [FigureType.Rook, FigureType.Queen]) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Bishop, myColor, currentBoard,
                    [FigureType.Queen, FigureType.Bishop]) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Knight, myColor, currentBoard,
                    [FigureType.Knight]) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.King, myColor, currentBoard,
                    [FigureType.King]) ||
                await IsKingCheckedBy<FigureType>(kingBlock, FigureType.Pawn, myColor, currentBoard,
                    [FigureType.Pawn, FigureType.Bishop, FigureType.King, FigureType.Queen]))
                response.Data.IsKingChecked = true;
            else
                response.Data.IsKingChecked = false;
            return response;
        }

        /// <summary>
        /// Generic helper used by the "king-as-attacker" technique.
        /// Temporarily places the requested figure type on the king's position and checks whether
        /// any enemy piece of the allowed types can be captured from there.
        /// Returns true if at least one such enemy piece exists → the king is in check.
        /// 
        /// Warning: Works correctly for Queen/Rook/Bishop/Knight/King.
        /// Warning: Does NOT work reliably for Pawn (asymmetric movement) – pawn checks must be handled separately.
        /// </summary>
        private async Task<bool> IsKingCheckedBy<TFigureType>(Block kingBlock, TFigureType figureType,
            FigureColors myColor, Board currentBoard, List<FigureType> figureTypes) where TFigureType : Enum
        {
            if (!Enum.IsDefined(typeof(TFigureType), figureType))
                return await Task.FromResult(false);
            var kingBlockClone = new Block
            {
                Position = kingBlock.Position,
                Figure = figureType switch
                {
                    FigureType.Rook => new Rook() { FigureColor = myColor },
                    FigureType.Bishop => new Bishop() { FigureColor = myColor },
                    FigureType.King => new King() { FigureColor = myColor },
                    FigureType.Knight => new Knight() { FigureColor = myColor },
                    FigureType.Pawn => new Pawn() { FigureColor = myColor },
                    FigureType.Queen => new Queen() { FigureColor = myColor },
                    _ => throw new ArgumentException()
                }
            };
            var possibleMovableAndCuttable =
                kingBlockClone.Figure.GetMovableAndCuttableBlocks(kingBlockClone.Position, currentBoard, kingBlockClone);

            var figuresForCheck = possibleMovableAndCuttable.CutableBlock.Where(block =>
                figureTypes.Contains<FigureType>(block.Figure.FigureType));

            var figureForChecks = figuresForCheck as Block[] ?? figuresForCheck.ToArray();

            if (!figureForChecks.Any())
                return await Task.FromResult(false);
            foreach (var figureForCheck in figureForChecks)
            {
                logger.LogInformation("King of color {Color} is in check by figure at position {Position}",
                    myColor, figureForCheck.Position);
            }

            return await Task.FromResult(true);
        }
    }
}