using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Core.Services.Services.Validations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
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
                    GetAllHistoryWidgetRequestDTO,
                    ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>,
                    ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>>
    {

        public async Task<ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>> Handle(
            GetAllHistoryWidgetsQuery<GetAllHistoryWidgetRequestDTO, ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var validationResult = await genericValidation.ValidateAsync(request.Request);
            if (!validationResult.IsValid)
                return ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                      null!, ChessGameResponseMessage.InvalidData,
                      HttpStatusCode.BadRequest);

            var connectionRequest = new ConnectionRequestDTO<GetAllHistoryWidgetRequestDTO>()
            {
                Data = request.Request
            };
            var result = await service.GetAllOpponents(connectionRequest);
            return ResponseDTO<GetAllHistoryWidgetsResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                new GetAllHistoryWidgetsResponseDTO()
                {
                    OpponentHistories = result.Data.OpponentHistories
                }, ChessGameResponseMessage.GameCreated, HttpStatusCode.Accepted);
        }
    }
}
