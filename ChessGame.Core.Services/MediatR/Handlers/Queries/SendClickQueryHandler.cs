using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Core.Services.Services.Validations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses;
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
            SendClickQuery<IRequestTypes<CanClickRequestDTO>,
                IResponseTypes<CanClickResponseDTO, ChessGameResponseMessage>>,
            IResponseTypes<CanClickResponseDTO, ChessGameResponseMessage>>
    {
        public async Task<IResponseTypes<CanClickResponseDTO, ChessGameResponseMessage>> Handle(
            SendClickQuery<IRequestTypes<CanClickRequestDTO>,
                IResponseTypes<CanClickResponseDTO, ChessGameResponseMessage>> request, 
            CancellationToken cancellationToken)
        {
            //Request Data
            var figureColor = request.Request.requestType.FigureColor;
            var currentBlock = request.Request.requestType.CurrentBlock;
            var previusBlockInformationDTO = request.Request.requestType.ClickedBlockInformationDto;
            var currentBoard = request.Request.requestType.CurrentBoardBoardState!;


            if ((int)figureColor !=
                (int)currentBoard.Turn)
            {
                logger.LogWarning("It's not the turn of player with color {Color}",
                    figureColor);

                return 
                    ChessGameResponse<CanClickResponseDTO>.CreateErrorResponse(
                        new CanClickResponseDTO()
                        {
                            ClickedBlock = null
                        },
                        ChessGameResponseMessage.InvalidMove,
                        HttpStatusCode.BadRequest,
                        ["It's not the turn of the player"]);
            }

            var currentBlockFromServer = currentBoard.GetBlockByPosition(currentBlock.Position);

            //if the current player is the same color as the figure on the clicked block and previusly clicked block is null
            if (currentBlock.Figure != null &&
                currentBlock.Figure.FigureColor == figureColor)
            {
                logger.LogInformation("Player with color {Color} clicked on their own figure at position {Position}",
                    figureColor,
                    currentBlock.Position);

                return await Task.FromResult(
                    ChessGameResponse<CanClickResponseDTO>.CreateSuccessResponse(
                        new CanClickResponseDTO()
                        {
                            ClickedBlock = currentBlockFromServer
                        },
                        ChessGameResponseMessage.MoveSuccessful, HttpStatusCode.Accepted,null));
            }

            //if the current player is clicked previusly and now clicked on a movable or cutable position

            if (previusBlockInformationDTO?.ClickedPosition == null ||
                (currentBlockFromServer.EventColor != EventColors.Cut &&
                 currentBlockFromServer.EventColor != EventColors.Move))
                return await Task.FromResult(
                    ChessGameResponse<CanClickResponseDTO>.CreateErrorResponse(
                        new CanClickResponseDTO()
                        {
                            ClickedBlock = null
                        },
                        ChessGameResponseMessage.InvalidMove,
                        HttpStatusCode.BadRequest,
                        ["It's not the turn of the player"]));

            logger.LogInformation("Player with color {Color} is attempting to move from {FromPosition} to {ToPosition}",
                figureColor,
                previusBlockInformationDTO.ClickedPosition,
                currentBlock.Position);

            return await Task.FromResult(
                ChessGameResponse<CanClickResponseDTO>.CreateSuccessResponse(
                    new CanClickResponseDTO()
                    {
                        ClickedBlock = currentBlockFromServer
                    },
                    ChessGameResponseMessage.MoveSuccessful, HttpStatusCode.Accepted,null));
        }   
    }
}
