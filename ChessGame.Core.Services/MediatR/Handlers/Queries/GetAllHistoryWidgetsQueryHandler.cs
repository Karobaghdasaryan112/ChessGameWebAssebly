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
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Queries
{
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
                    IRequestTypes<GetAllHistoryWidgetRequestDTO>,
                    IResponseTypes<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>,
                    IResponseTypes<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>
    {

        public async Task<IResponseTypes<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>> Handle(
            GetAllHistoryWidgetsQuery<IRequestTypes<GetAllHistoryWidgetRequestDTO>,
                IResponseTypes<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var validationResult = await genericValidation.ValidateAsync(request.Request.requestType);
            if (!validationResult.IsValid)
                ChessGameResponse<GetAllHistoryWidgetsResponseDTO>.CreateSuccessResponse(
                    null!, ChessGameResponseMessage.InvalidData,
                    HttpStatusCode.BadRequest, null!);

            throw new NotImplementedException();
        }
    }
}
