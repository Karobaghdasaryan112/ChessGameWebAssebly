using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Commands
{
    public class BoardInitializeCommandHandler(
        IValidator<BoardInitializeRequestDTO> validator,
        ILogger<BoardInitializeCommandHandler> logger,
        IBoardService service) :
        MediatR_Base<BoardInitializeRequestDTO, BoardInitializeCommandHandler, IBoardService>(
            validator,
            logger,
            service),
        IRequestHandler<
            BoardInitializeCommand<IRequestTypes<BoardInitializeRequestDTO>,
                IResponseTypes<BoardInitializeResponseDTO, ChessGameResponseMessage>>,
            IResponseTypes<BoardInitializeResponseDTO, ChessGameResponseMessage>>
    {
        public async Task<IResponseTypes<BoardInitializeResponseDTO, ChessGameResponseMessage>> Handle(
            BoardInitializeCommand<IRequestTypes<BoardInitializeRequestDTO>,
                IResponseTypes<BoardInitializeResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.RequestDTO.requestType, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errorMessages = 
                    validationResult.
                    Errors.
                    Select(error => error.ErrorMessage).
                    ToList();

                return ChessGameResponse<BoardInitializeResponseDTO>.CreateErrorResponse(
                    ChessGameResponseMessage.GameCreationFailed,
                    HttpStatusCode.BadRequest, 
                    errorMessages);
            }

            var connectionRequestDto = new ConnectionRequestDTO<BoardInitializeRequestDTO>()
            {
                Data = request.RequestDTO.requestType
            };

            var initializeGameResponseDTO = await _service.InitializeBoardAsync(connectionRequestDto);

            if (!initializeGameResponseDTO.IsSuccess)
            {
                return ChessGameResponse<BoardInitializeResponseDTO>.CreateErrorResponse(
                    initializeGameResponseDTO.Message,
                    initializeGameResponseDTO.HttpStatusCode, initializeGameResponseDTO.Errors);
            }

            var BoardInitialize = new Board(default(FigureColors));

            var addingResult = ActiveGames.AddGame(initializeGameResponseDTO.Data.GameId, BoardInitialize);

            if (!addingResult)
            {
                _logger.LogError("Failed to add the new game with ID {GameId} to active games.",
                    initializeGameResponseDTO.Data.GameId);
                return ChessGameResponse<BoardInitializeResponseDTO>.CreateErrorResponse(
                    ChessGameResponseMessage.GameCreationFailed,
                    HttpStatusCode.InternalServerError,
                    ["Field To Add Game Into Active Games"]);
            }

            var responseData = new BoardInitializeResponseDTO()
            { board = BoardInitialize, GameId = initializeGameResponseDTO.Data.GameId };


            return ChessGameResponse<BoardInitializeResponseDTO>.CreateSuccessResponse(
                responseData,
                ChessGameResponseMessage.GameCreated,
                HttpStatusCode.Created, null);
        }
    }
}
