using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Core.Services.Services.Validations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Queries
{
    /// <summary>
    /// Handles queries for retrieving the history of a chess game, including validation and response formatting.
    /// </summary>
    /// <remarks>This handler coordinates validation and data retrieval for game history queries. It returns a
    /// response indicating success or failure, along with any relevant data or error messages. The handler is typically
    /// used within a MediatR pipeline to process requests for chess game history.</remarks>
    /// <param name="validator">The validator used to ensure that the incoming game history request meets all required criteria.</param>
    /// <param name="logger">The logger instance used for recording diagnostic and operational information during query handling.</param>
    /// <param name="service">The board service used to access and retrieve game history data.</param>
    /// <param name="validationService">The service responsible for performing additional validation on the request data.</param>
    public class GetGameHistoryQueryHandler(
        IValidator<GetGameHistoryRequestDTO> validator,
        ILogger<GetGameHistoryQueryHandler> logger,
        IBoardService service,
        GenericValidationService validationService) : MediatR_Base<
            GetGameHistoryRequestDTO,
            GetGameHistoryQueryHandler,
            IBoardService>(validator, logger, service),
        IRequestHandler<
            GetGameHistoryQuery<GetGameHistoryRequestDTO,
                ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>>,
            ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>>
    {

        /// <summary>
        /// Handles the retrieval of chess game history based on the specified query request.
        /// </summary>
        /// <param name="request">The query containing the request data for retrieving game history. Must not be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A response object containing the game history data if the request is valid and successful; otherwise, an
        /// error response with the appropriate status and message.</returns>
        public async Task<ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>> Handle(
            GetGameHistoryQuery<GetGameHistoryRequestDTO, ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var validationResult = await validationService.ValidateAsync(request.RequestType);
            if (!validationResult.IsValid)
                return ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                      default(GetGameHistoryResponseDTO)!, ChessGameResponseMessage.InvalidData,
                      HttpStatusCode.BadRequest);

            var gameHistoryResult = await service.GetGameHistoryAsync(
                new GetGameHistoryRequestDTO()
                {
                    GameId = request.RequestType.GameId
                });

            return !gameHistoryResult.IsSuccess ? ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>.
                CreateErrorResponse(
                    gameHistoryResult.Data,
                    gameHistoryResult.Message,
                    gameHistoryResult.HttpStatusCode,
                    gameHistoryResult.Errors) :
                ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                    gameHistoryResult.Data,
                    gameHistoryResult.Message,
                    gameHistoryResult.HttpStatusCode);
        }
    }
}
