using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Core.Services.Services.Validations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using SharedResources.Validation.ChessGameValidations.RequestValidations.HistoryWidgetsRequests;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Queries
{
    public class GetHistoryWidgetsPaginationQueryHandler(
        IValidator<GetAllHistoryWidgetsRequestDTOValidator> validator,
        ILogger<GetHistoryWidgetsPaginationQueryHandler> logger,
        IHistoryWidgetService service,
        GenericValidationService genericValidation) :
        MediatR_Base<
            GetAllHistoryWidgetsRequestDTOValidator,
            GetHistoryWidgetsPaginationQueryHandler,
            IHistoryWidgetService>(validator, logger, service)
        , IRequestHandler<
            GetHistoryWidgetsPaginationQuery<
                IRequestTypes<GetGamesByCurrentAndOpponentIdsPaginationRequestDTO>,
                IResponseTypes<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>>,
            IResponseTypes<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>>
    {

        public async Task<IResponseTypes<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>>
            Handle(GetHistoryWidgetsPaginationQuery<IRequestTypes<GetGamesByCurrentAndOpponentIdsPaginationRequestDTO>,
                IResponseTypes<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO, ChessGameResponseMessage>> request, CancellationToken cancellationToken)
        {
            var validationResult = await genericValidation.ValidateAsync(request.Request.requestType);
            if (!validationResult.IsValid)
                return ChessGameResponse<GetGamesByCurrentAndOpponentIdsPaginationResponseDTO>.CreateErrorResponse(
                    ChessGameResponseMessage.InvalidData,
                    HttpStatusCode.BadRequest,
                    validationResult.
                        Errors.
                        Select(error =>
                            error.ErrorMessage).
                        ToList());

            return await service.
                GetGamesByCurrentAndOpponentIdsPagination(request.Request);
        }
    }
}
