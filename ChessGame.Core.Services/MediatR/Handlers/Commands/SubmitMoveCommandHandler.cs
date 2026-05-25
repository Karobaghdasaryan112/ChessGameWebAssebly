using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Extentions;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Enums.Orientations;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.ChessGameResourceContracts;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;
using System.Net;
using SharedResources.ChessGameResource.Figures;

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
            // Extracting necessary data from the request
            var fromPosition = request.RequestDTO.From;
            var toPosition = request.RequestDTO.To;
            var gameId = request.RequestDTO.GameId;
            var currentBoardState = request.RequestDTO.CurrentBoardState;
            var promotionFigure = request.RequestDTO.PromotionFigure;

            // Retrieving the blocks corresponding to the from and to positions
            var fromBlock = currentBoardState.GetBlockByPosition(fromPosition!);
            var toBlock = currentBoardState.GetBlockByPosition(toPosition!);
            WriteSubmitMoveLogger("Submit Move Before move", gameId, fromPosition, toPosition, fromBlock, toBlock);

            var response = ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new SubmitMoveResponseDTO()
                {
                    IsKingChecked = false, IsKingMate = false, IsMoveSuccess = true
                }, ChessGameResponseMessage.MoveSuccessful,
                HttpStatusCode.OK,
                null!);

            // Checking if there is a figure at the From position
            if (fromBlock?.Figure == null)
                return CheckFromBlock(fromPosition!, fromBlock!, toBlock!, gameId, response);

            // Swapping the figures between the from and to blocks to simulate the move

            var toBlockTemp = (Block)toBlock.Clone();
            if (promotionFigure != default)
            {
                var icrement = fromBlock.Figure.FigureColor == FigureColors.Black ? +1 : -1;
                var realToBlock = currentBoardState.GetBlockByPosition(
                    toBlockTemp.Position.VerticalOrientation + icrement, toBlock.Position.HorizontalOrientation);
                toBlock = realToBlock;
                toBlockTemp = (Block)realToBlock.Clone();
                toBlock.Figure = CreatePromotionFigure(promotionFigure, fromBlock.Figure.FigureColor);
            }
            else
            {
                toBlock.Figure = fromBlock.Figure;
            }

            fromBlock.Figure = null!;


            WriteSubmitMoveLogger("Submit Move After move", gameId, fromPosition, toPosition, fromBlock, toBlock);

            // Handle castling logic
            if (toBlock.EventColor == EventColors.Castle)
            {
                //Short or Long castle(from Getting Rook)
                int kingincrement = 0;
                int rookIncrement = 0;
                var isShort = (int)toBlock.Position.HorizontalOrientation > 4 ? 7 : 0;

                if (isShort == 7)
                {
                    kingincrement = 1;
                    rookIncrement = 1;
                }
                else
                {
                    kingincrement = -1;
                    rookIncrement = -2;
                }

                var rookBlock = currentBoardState.GetBlockByPosition(toBlock.Position.VerticalOrientation,
                    (HorizontalOrientation)((int)toBlock.Position.HorizontalOrientation + rookIncrement));

                var rookNewPosition = new Position(toBlock.Position.VerticalOrientation,
                    (HorizontalOrientation)((int)toBlock.Position.HorizontalOrientation - kingincrement));

                var rookNewBlock = currentBoardState.GetBlockByPosition(rookNewPosition!);
                response.Data.CastlingRookPositions = new CastlingRookPositions()
                {
                    RookFrom = rookBlock.Position,
                    RookTo = rookNewPosition
                };
                SwithFigures(rookBlock, rookNewBlock!, default(FigureType));
            }

            // Resetting any eventable blocks on the board before processing the move
            request.RequestDTO.CurrentBoardState.ResetEventableBlocks();

            logger.LogInformation("Move submitted in game {GameId} from {FromPosition} to {ToPosition}", gameId,
                fromPosition, toPosition);

            var requestQuery = new IsKingCheckedRequestDTO()
            {
                ChosenColor = currentBoardState.Turn,
                CurrentBoard = currentBoardState
            };

            var query =
                new IsKingCheckedQuery<IsKingCheckedRequestDTO,
                    ResponseDTO<IsKingCheckedResponseDTO, ChessGameResponseMessage>>(requestQuery);

            // Checking if the king is in check after the move
            var isKingCheckedResult = await mediator.Send(query, cancellationToken);

            // If the king is not in check, the move is successful
            if (isKingCheckedResult.IsSuccess && !isKingCheckedResult.Data.IsKingChecked)
            {
                toBlock.Figure.IsMoves = true;
                return response;
            }

            // If the king is in check, revert the move and update the response accordingly

            logger.LogWarning("Move from {FromPosition} to {ToPosition} in game {GameId} would leave king in check",
                fromPosition, toPosition, gameId);

            RevertMove(fromBlock!, toBlock!, toBlockTemp.Figure!);

            WriteSubmitMoveLogger("Revert Move After move", gameId, fromPosition, toPosition, fromBlock, toBlock);

            response.Data.IsKingChecked = true;

            logger.LogInformation("Move revert in game {GameId} from {FromPosition} to {ToPosition}", gameId,
                fromPosition, toPosition);

            return response;
        }

        //private methods
        private ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage> CheckFromBlock(Position fromPosition,
            Block fromBlock, Block toBlock, Guid gameId,
            ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage> responseDTO)
        {
            logger.LogWarning("No figure found at position {Position} in game {GameId}", fromPosition, gameId);

            responseDTO.Data.IsMoveSuccess = false;
            responseDTO.Errors =
            [
                $"If there is no figure at the from-{fromBlock.Position.VerticalOrientation}{fromBlock.Position.HorizontalOrientation} position"
            ];
            responseDTO.IsSuccess = false;
            return responseDTO;
        }

        private Block SwithFigures(Block fromBlock, Block toBlock, FigureType promotionFigure)
        {
            var toBlockTemp = (Block)toBlock.Clone();
            toBlock.Figure = promotionFigure != default
                ? CreatePromotionFigure(promotionFigure, toBlockTemp.Figure.FigureColor)
                : fromBlock.Figure;

            fromBlock.Figure = null!;
            return toBlockTemp!;
        }

        private static IFigure CreatePromotionFigure(FigureType promotionFigure, FigureColors color)
        {
            return promotionFigure switch
            {
                FigureType.Queen => new Queen { FigureColor = color },
                FigureType.Rook => new Rook { FigureColor = color },
                FigureType.Bishop => new Bishop { FigureColor = color },
                FigureType.Knight => new Knight { FigureColor = color },
                FigureType.Pawn => new Pawn { FigureColor = color },
                _ => new Queen { FigureColor = color }
            };
        }

        private void RevertMove(Block fromBlock, Block toBlock, IFigure toBlockTemp)
        {
            fromBlock.Figure = toBlock.Figure;
            toBlock.Figure = toBlockTemp;
        }

        private void WriteSubmitMoveLogger(string eventTime, Guid gameId, Position fromPosition, Position toPosition,
            Block fromBlock, Block toBlock)
        {
            Console.WriteLine(eventTime);
            Console.WriteLine($"GameId: {gameId}");
            Console.WriteLine($"FromPosition: {fromPosition}");
            Console.WriteLine($"ToPosition: {toPosition}");
            Console.WriteLine($"FromBlock FIgureType: {fromBlock?.Figure?.FigureType}:");
            Console.WriteLine($"FromBlock FIgureType: {fromBlock?.Figure?.FigureType}:");
            Console.WriteLine($"ToBlock FIgureType: {toBlock?.Figure?.FigureType}:");
        }
    }
}