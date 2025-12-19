using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Core.Services.Services.Validations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Queries
{
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
        public async Task<ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>> Handle(SendClickQuery<CanClickRequestDTO, ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>> request, CancellationToken cancellationToken)
        {
            //Request Data
            var figureColor = request.Request.FigureColor;
            var currentBlock = request.Request.CurrentBlock;
            var previusBlockInformationDTO = request.Request.ClickedBlockInformationDto;
            var currentBoard = request.Request.CurrentBoardBoardState!;

            var successResponse = ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                        new CanClickResponseDTO() { },
                        ChessGameResponseMessage.MoveSuccessful, HttpStatusCode.Accepted);

            var errorResponse = ResponseDTO<CanClickResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(
                        new CanClickResponseDTO()
                        {
                            ClickedBlock = null
                        },
                        ChessGameResponseMessage.InvalidMove,
                        HttpStatusCode.BadRequest,
                        ["It's not the turn of the player"]);

            if ((int)figureColor !=
                (int)currentBoard.Turn)
            {
                logger.LogWarning("It's not the turn of player with color {Color}",
                    figureColor);

                return errorResponse;
            }

            var currentBlockFromServer = currentBoard.GetBlockByPosition(currentBlock.Position);

            //if the current player is the same color as the figure on the clicked block and previusly clicked block is null
            if (currentBlock.Figure != null &&
                currentBlock.Figure.FigureColor == figureColor)
            {
                logger.LogInformation("Player with color {Color} clicked on their own figure at position {Position}",
                    figureColor,
                    currentBlock.Position);


                successResponse.Data.ClickedBlock = currentBlockFromServer;

                return successResponse;
            }

            //if the current player is clicked previusly and now clicked on a movable or cutable position

            if (previusBlockInformationDTO?.ClickedPosition == null ||
                (currentBlockFromServer.EventColor != EventColors.Cut &&
                 currentBlockFromServer.EventColor != EventColors.Move))
                return errorResponse;

            logger.LogInformation("Player with color {Color} is attempting to move from {FromPosition} to {ToPosition}",
                figureColor,
                previusBlockInformationDTO.ClickedPosition,
                currentBlock.Position);


            successResponse.Data.ClickedBlock = currentBlockFromServer;

            return successResponse;

        }
    }
}
