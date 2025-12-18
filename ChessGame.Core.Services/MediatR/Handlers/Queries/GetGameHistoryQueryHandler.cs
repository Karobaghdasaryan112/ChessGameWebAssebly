using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using ChessGame.Core.Services.Services.Validations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Queries
{
    public class GetGameHistoryQueryHandler(
        IValidator<GetGameHistoryRequestDTO> validator,
        ILogger<GetGameHistoryQueryHandler> logger,
        IBoardService service,
        GenericValidationService validationService) : MediatR_Base<
            GetGameHistoryRequestDTO,
            GetGameHistoryQueryHandler,
            IBoardService>(validator, logger, service),
        IRequestHandler<
            GetGameHistoryQuery<IRequestTypes<GetGameHistoryRequestDTO>,
                IResponseTypes<GetGameHistoryResponseDTO, ChessGameResponseMessage>>,
            IResponseTypes<GetGameHistoryResponseDTO, ChessGameResponseMessage>>
    {
        public async Task<IResponseTypes<GetGameHistoryResponseDTO, ChessGameResponseMessage>>
            Handle(
            GetGameHistoryQuery<IRequestTypes<GetGameHistoryRequestDTO>,
                IResponseTypes<GetGameHistoryResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {

            var validationResult = await validationService.ValidateAsync(request.RequestType.requestType);
            if (!validationResult.IsValid)
                return ChessGameResponse<GetGameHistoryResponseDTO>.CreateSuccessResponse(
                      null!, ChessGameResponseMessage.InvalidData,
                      HttpStatusCode.BadRequest, null!);

            var gameHistoryResult = await service.GetGameHistoryAsync(new ConnectionRequestDTO<GetGameHistoryRequestDTO>()
            {
                Data = new GetGameHistoryRequestDTO()
                {
                    GameId = request.RequestType.requestType.GameId
                }
            });
            return !gameHistoryResult.IsSuccess ? ChessGameResponse<GetGameHistoryResponseDTO>.
                CreateErrorResponse(
                    gameHistoryResult.Data,
                    gameHistoryResult.Message,
                    gameHistoryResult.HttpStatusCode,
                    gameHistoryResult.Errors) :
                ChessGameResponse<GetGameHistoryResponseDTO>.CreateSuccessResponse(
                    gameHistoryResult.Data,
                    gameHistoryResult.Message,
                    gameHistoryResult.HttpStatusCode,
                    gameHistoryResult.Errors);
        }
    }
}
