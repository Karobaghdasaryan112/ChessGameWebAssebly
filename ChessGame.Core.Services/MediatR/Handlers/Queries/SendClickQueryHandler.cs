using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Core.Services.Services.Validations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Enums.FigureTypes;
using SharedResources.ChessGameResource.Enums.Orientations;
using SharedResources.ChessGameResource.Models;
using SharedResources.DTOs.ChessGameDTOs.ChessGameSharedDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Queries
{
    /// <summary>
    /// Handles queries for validating and processing click actions on a chess board, determining whether a move is
    /// allowed and returning the appropriate response.
    /// </summary>
    /// <remarks>This handler is typically used within a CQRS and MediatR-based architecture to process user
    /// interactions with a chess game board. It ensures that moves are only allowed when it is the correct player's
    /// turn and that the move adheres to the game's rules. Logging is performed for both successful and invalid move
    /// attempts.</remarks>
    /// <param name="validator">The validator used to ensure that the incoming request data meets all required validation rules.</param>
    /// <param name="logger">The logger instance used to record informational and warning messages during query handling.</param>
    /// <param name="service">The board service that provides access to chess board operations and state management.</param>
    /// <param name="mediator">The mediator used to coordinate communication between components and dispatch related requests.</param>
    /// <param name="genericValidation">The generic validation service used for additional validation logic beyond the standard request validator.</param>
    public class SendClickQueryHandler(
        IValidator<CanClickRequestDTO> validator,
        ILogger<SendClickQueryHandler> logger,
        IBoardService service,
        IMediator mediator,
    GenericValidationService genericValidation)
        : MediatR_Base<CanClickRequestDTO, SendClickQueryHandler, IBoardService>(validator, logger, service),
        IRequestHandler<
            SendClickQuery<CanClickRequestDTO,
                ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>>,
            ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>>
    {

        /// <summary>
        /// Processes a click event in a chess game and determines whether the action is valid based on the current game
        /// state.
        /// </summary>
        /// <remarks>Returns a success response if the player clicks on their own piece or makes a valid
        /// move according to the current turn. Returns an error response if the action is not allowed, such as when it
        /// is not the player's turn or the move is invalid.</remarks>
        /// <param name="request">The query containing the details of the click event, including the player's color, the selected block,
        /// previous click information, and the current board state. Cannot be null.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response indicating whether
        /// the click is valid and includes a message describing the outcome.</returns>
        public async Task<ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>> Handle(
            SendClickQuery<CanClickRequestDTO, ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var figureColor = request.Request.FigureColor;
            var currentBlock = request.Request.CurrentBlock;
            var previusBlockInformationDTO = request.Request.ClickedBlockInformationDto;
            var currentBoard = request.Request.CurrentBoardBoardState!;

            var successResponse = ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(new CanClickResponseDTO() { CastlingInfosDTO = new List<CastlingInfosDTO>() }, ChessGameResponseMessage.MoveSuccessful, HttpStatusCode.Accepted);

            var errorResponse = ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        new CanClickResponseDTO()
                        {
                            ClickedBlock = null!
                        },
                        ChessGameResponseMessage.InvalidMove,
                        HttpStatusCode.BadRequest,
                        ["It's not the turn of the player"]);

            if ((int)figureColor != (int)currentBoard.Turn)
            {
                logger.LogWarning("It's not the turn of player with color {Color}", figureColor);

                return errorResponse;
            }

            var currentBlockFromServer = currentBoard.GetBlockByPosition(currentBlock!.Position);

            if (currentBlock.Figure != null && currentBlock.Figure.FigureColor == figureColor)
            {
                logger.LogInformation("Player with color {Color} clicked on their own figure at position {Position}", figureColor, currentBlock.Position);

                var castlingResult = await CastlingMoveLogic(currentBlockFromServer, figureColor, currentBoard);
                if (castlingResult.Any(c => c.IsCastling))
                {
                    logger.LogInformation("Player with color {Color} is attempting to castle from {FromPosition} to {ToPosition}", figureColor, previusBlockInformationDTO.ClickedPosition, currentBlock.Position);
                    successResponse.Data.ClickedBlock = currentBlockFromServer;
                    successResponse.Data.CastlingInfosDTO = castlingResult;
                    return successResponse;
                }

                successResponse.Data.ClickedBlock = currentBlockFromServer;

                return successResponse;
            }

            if (previusBlockInformationDTO?.ClickedPosition == null || (currentBlockFromServer.EventColor != EventColors.Cut && currentBlockFromServer.EventColor != EventColors.Move))
                return errorResponse;



            logger.LogInformation("Player with color {Color} is attempting to move from {FromPosition} to {ToPosition}", figureColor, previusBlockInformationDTO.ClickedPosition, currentBlock.Position);

            successResponse.Data.ClickedBlock = currentBlockFromServer;

            return successResponse;
        }

        //private methods
        private async Task<List<CastlingInfosDTO>> CastlingMoveLogic(Block currentBlock, FigureColors figureColor, Board currentBoard)
        {
            var castlingInfos = new List<CastlingInfosDTO>();
            if (currentBlock.Figure?.FigureType == FigureType.King && !currentBlock.Figure.IsMoves)
            {
                var rooks = currentBoard.GetBlockByFigureTypeAndColor(FigureType.Rook, figureColor);
                var shortCastlingDirection = 3;
                var longCastlingDirection = 4;
                var kingPosition = currentBlock.Position;
                foreach (var rook in rooks)
                {
                    if (!rook.Figure.IsMoves)
                    {
                        if (Math.Abs(rook.Position.HorizontalOrientation - currentBlock.Position.HorizontalOrientation) == shortCastlingDirection ||
                           Math.Abs(rook.Position.HorizontalOrientation - currentBlock.Position.HorizontalOrientation) == longCastlingDirection)
                        {
                            if (IsPathClearForCastling(currentBoard, currentBlock.Position, rook.Position))
                            {
                                if (await IsKingNotInCheckAfterCastling(currentBoard, figureColor, kingPosition, rook.Position))
                                {
                                    castlingInfos.Add(new CastlingInfosDTO
                                    {
                                        IsShortCastle = rook.Position.HorizontalOrientation > kingPosition.HorizontalOrientation,
                                        IsCastling = true,
                                        CastlingPosition = rook.Position.HorizontalOrientation > kingPosition.HorizontalOrientation
                                            ? new Position(kingPosition.VerticalOrientation, HorizontalOrientation.g) //Castle position to paint
                                            : new Position(kingPosition.VerticalOrientation, HorizontalOrientation.c)
                                        //Castle position to paint
                                    });
                                }
                            }
                        }
                    }
                }
                castlingInfos.ForEach(castl =>
                {
                    var castlingBlock = currentBoard.GetBlockByPosition(castl.CastlingPosition);
                    castlingBlock.EventColor = EventColors.Castle;
                });

            }
            return castlingInfos;
        }
        private bool IsPathClearForCastling(Board board, Position kingPosition, Position rookPosition)
        {
            int step = rookPosition.HorizontalOrientation > kingPosition.HorizontalOrientation ? 1 : -1;
            for (int col = (int)kingPosition.HorizontalOrientation + step; col != (int)rookPosition.HorizontalOrientation; col += step)
            {
                var block = board.GetBlockByPosition(new Position(kingPosition.VerticalOrientation, (HorizontalOrientation)col));
                if (block.Figure != null)
                {
                    return false; // Path is not clear
                }
            }
            return true; // Path is clear
        }

        private async Task<bool> IsKingNotInCheckAfterCastling(Board board, FigureColors figureColor, Position kingPosition, Position rookPosition)
        {
            int step = rookPosition.HorizontalOrientation > kingPosition.HorizontalOrientation ? 1 : -1;
            var cloneBoard = (Board)board.Clone();
            for (int col = (int)kingPosition.HorizontalOrientation + step; col != (int)rookPosition.HorizontalOrientation; col += step)
            {

                var nextBlock = cloneBoard.GetBlockByPosition(new Position(kingPosition.VerticalOrientation, (HorizontalOrientation)col));
                var submitMoveCommand = new SubmitMoveCommand<SubmitMoveRequestDTO, ResponseDTO<SubmitMoveResponseDTO, ChessGameResponseMessage>>(
                    new SubmitMoveRequestDTO()
                    {
                        From = kingPosition,
                        To = nextBlock.Position,
                        CurrentBoardState = cloneBoard,
                        GameId = Guid.Empty

                    });
                var canMove = await mediator.Send(submitMoveCommand);

                kingPosition = nextBlock.Position;

                if (canMove.Data.IsKingChecked || !canMove.IsSuccess)
                {
                    return false; // King would be in check
                }
            }
            return true; // King is safe
        }
    }
}

