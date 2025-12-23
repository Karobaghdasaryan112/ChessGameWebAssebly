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
    /// Handles requests to retrieve a paginated list of chess games between the current user and a specified opponent,
    /// performing validation and returning the results with an appropriate response message.
    /// </summary>
    /// <remarks>If the request data is invalid, the handler returns a response indicating the validation
    /// errors. This handler integrates validation, logging, and data retrieval to ensure robust processing of paginated
    /// game history queries.</remarks>
    /// <param name="validator">The validator used to ensure that the pagination request data meets all required criteria before processing.</param>
    /// <param name="logger">The logger instance used for recording diagnostic and operational information during request handling.</param>
    /// <param name="service">The service responsible for retrieving chess game history data based on the provided criteria.</param>
    /// <param name="genericValidation">The generic validation service used to perform additional validation on the request data.</param>
    public class GetHistoryWidgetsPaginationQueryHandler(
        IValidator<GetGamesByCurrentAndOpponentIdsPaginationRequestDTO> validator,
        ILogger<GetHistoryWidgetsPaginationQueryHandler> logger,
        IHistoryWidgetService service,
        GenericValidationService genericValidation) :
        MediatR_Base<
            GetGamesByCurrentAndOpponentIdsPaginationRequestDTO,
            GetHistoryWidgetsPaginationQueryHandler,
            IHistoryWidgetService>(validator, logger, service)
        , IRequestHandler<
            GetHistoryWidgetsPaginationQuery<
                GetGamesByCurrentAndOpponentIdsPaginationRequestDTO,
                ResponseDTO<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>>,
            ResponseDTO<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>>
    {
        /// <summary>
        /// Handles a request to retrieve a paginated list of chess games between the current user and a specified
        /// opponent.
        /// </summary>
        /// <remarks>If the request data is invalid, the response will indicate an error with details
        /// about the validation failures.</remarks>
        /// <param name="request">An object containing the pagination and filtering criteria for retrieving games by current and opponent user
        /// IDs.</param>
        /// <param name="cancellationToken">A token that can be used to request cancellation of the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response with the paginated
        /// list of games and a response message indicating the outcome.</returns>
        public async Task<ResponseDTO<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>> Handle(
            GetHistoryWidgetsPaginationQuery<GetGamesByCurrentAndOpponentIdsPaginationRequestDTO, ResponseDTO<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var validationResult = await genericValidation.ValidateAsync(request.Request);
            if (!validationResult.IsValid)
                return ResponseDTO<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>.CreateErrorResponse(null!,ChessGameResponseMessage.InvalidData,HttpStatusCode.BadRequest,
                    validationResult.Errors.Select(error =>error.ErrorMessage).ToList());

            return await service.GetGamesByCurrentAndOpponentIdsPagination(request.Request);
        }
    }
}
