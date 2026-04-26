using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Queries;
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
    /// Handles queries to retrieve all history widgets, performing validation and coordinating with the history widget
    /// service.
    /// </summary>
    /// <param name="validator">The validator used to ensure that incoming request DTOs meet required criteria before processing.</param>
    /// <param name="logger">The logger instance used for recording diagnostic and operational information during query handling.</param>
    /// <param name="service">The service responsible for accessing and retrieving history widget data.</param>
    /// <param name="genericValidation">The generic validation service used to perform additional validation on the request data.</param>
    public class GetAllHistoryWidgetsQueryHandler(
        IValidator<GetAllHistoryWidgetRequestDTO> validator,
        ILogger<GetAllHistoryWidgetsQueryHandler> logger,
        IHistoryWidgetService service,
        GenericValidationService genericValidation)
        : MediatR_Base<
                GetAllHistoryWidgetRequestDTO,
            GetAllHistoryWidgetsQueryHandler,
            IHistoryWidgetService>(validator, logger, service),
            IRequestHandler<
                GetAllHistoryWidgetsQuery<
                    GetAllHistoryWidgetRequestDTO,
                    ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>,
                    ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>
    {
        /// <summary>
        /// Handles the retrieval of all history widgets for a chess game based on the specified query request.
        /// </summary>
        /// <remarks>If the request data is invalid, the response will indicate an error with an
        /// appropriate status message and HTTP status code.</remarks>
        /// <param name="request">The query containing the request data for retrieving all history widgets. Must not be null.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a response object with the
        /// retrieved history widgets data and a status message indicating the outcome of the operation.</returns>
        public async Task<ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>> Handle(
            GetAllHistoryWidgetsQuery<GetAllHistoryWidgetRequestDTO, ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var validationResult = await genericValidation.ValidateAsync(request.Request);
            if (!validationResult.IsValid)
                return ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                      default(GetAllHistoryWidgetsResponseDTO)!, ChessGameResponseMessage.InvalidData,
                      HttpStatusCode.BadRequest);


            var result = await service.GetAllOpponents(request.Request);
            return ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new GetAllHistoryWidgetsResponseDTO()
                {
                    OpponentHistories = result.Data.OpponentHistories
                }, ChessGameResponseMessage.GameCreated, HttpStatusCode.Accepted);
        }
    }
}
