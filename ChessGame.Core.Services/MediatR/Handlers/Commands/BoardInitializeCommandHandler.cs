using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Commands
{
    /// <summary>
    /// Handles commands to initialize a new chess board and game session, including validation, board creation, and
    /// registration of the game in the active games collection.
    /// </summary>
    /// <remarks>This handler coordinates the process of starting a new chess game, including input
    /// validation, board setup, and tracking the new game instance. It is typically used within a MediatR pipeline to
    /// process board initialization requests.</remarks>
    /// <param name="validator">The validator used to ensure that the board initialization request data meets all required criteria.</param>
    /// <param name="logger">The logger used to record informational and error messages during the handling of board initialization commands.</param>
    /// <param name="service">The service responsible for performing board initialization and related game setup operations.</param>
    public class BoardInitializeCommandHandler(IValidator<BoardInitializeRequestDTO> validator, ILogger<BoardInitializeCommandHandler> logger, IBoardService service) :
        MediatR_Base<BoardInitializeRequestDTO, BoardInitializeCommandHandler, IBoardService>(validator, logger, service),
        IRequestHandler<
            BoardInitializeCommand<BoardInitializeRequestDTO, ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>>,
            ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>>
    {

        /// <summary>
        /// Handles a board initialization command by validating the request, initializing a new chess game board, and
        /// returning the result as a response DTO.
        /// </summary>
        /// <remarks>If the request data is invalid or the board cannot be initialized, the response
        /// contains error messages and an error status code. The method also attempts to add the new game to the
        /// collection of active games and logs an error if this fails.</remarks>
        /// <param name="request">The command containing the board initialization request data and the expected response type.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A response DTO containing the result of the board initialization operation. If successful, the response
        /// includes the initialized board and game ID; otherwise, it contains error information and an appropriate
        /// status code.</returns>
        public async Task<ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>> Handle(
            BoardInitializeCommand<BoardInitializeRequestDTO,
            ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.RequestDTO, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

                return ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(ChessGameResponseMessage.GameCreationFailed, HttpStatusCode.BadRequest, errorMessages);
            }

            var initializeGameResponseDTO = await _service.InitializeBoardAsync(request.RequestDTO);

            if (!initializeGameResponseDTO.IsSuccess)
            {
                return ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(initializeGameResponseDTO.Message, initializeGameResponseDTO.HttpStatusCode, initializeGameResponseDTO.Errors);
            }

            var BoardInitialize = new Board(default(FigureColors));

            var addingResult = ActiveGames.AddGame(initializeGameResponseDTO.Data.GameId, BoardInitialize);

            if (!addingResult)
            {
                _logger.LogError("Failed to add the new game with ID {GameId} to active games.", initializeGameResponseDTO.Data.GameId);
                return ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(ChessGameResponseMessage.GameCreationFailed, HttpStatusCode.InternalServerError, ["Field To Add Game Into Active Games"]);
            }

            var responseData = new BoardInitializeResponseDTO()
            {
                board = BoardInitialize,
                GameId = initializeGameResponseDTO.Data.GameId
            };

            return ResponseDTO<BoardInitializeResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(responseData, ChessGameResponseMessage.GameCreated, HttpStatusCode.Created, null!);
        }
    }
}
