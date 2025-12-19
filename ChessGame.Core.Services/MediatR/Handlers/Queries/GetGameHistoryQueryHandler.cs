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

        public async Task<ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>> Handle(
            GetGameHistoryQuery<GetGameHistoryRequestDTO, ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var validationResult = await validationService.ValidateAsync(request.RequestType);
            if (!validationResult.IsValid)
                return ResponseDTO<GetGameHistoryResponseDTO, ChessGameResponseMessage>.CreateSuccessResponse(
                      null!, ChessGameResponseMessage.InvalidData,
                      HttpStatusCode.BadRequest);

            var gameHistoryResult = await service.GetGameHistoryAsync(new ConnectionRequestDTO<GetGameHistoryRequestDTO>()
            {
                Data = new GetGameHistoryRequestDTO()
                {
                    GameId = request.RequestType.GameId
                }
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
