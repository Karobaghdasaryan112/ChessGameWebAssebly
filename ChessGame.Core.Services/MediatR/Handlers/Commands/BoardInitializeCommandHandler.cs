using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Enums.Colors;
using SharedResources.ChessGameResource.Models;
using SharedResources.ChessGameResource.StaticResources;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.MediatRRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using System.Net;

namespace ChessGame.Core.Services.MediatR.Handlers.Commands
{
    public class BoardInitializeCommandHandler :
        MediatR_Base<BoardInitializeRequestDTO, BoardInitializeCommandHandler, IBoardService>,
        IRequestHandler<
            BoardInitializeCommand<IRequestTypes<BoardInitializeRequestDTO>, IResponseTypes<BoardInitializeResponseDTO, ChessGameResponseMessage>>,
            IResponseTypes<BoardInitializeResponseDTO, ChessGameResponseMessage>>
    {

        public BoardInitializeCommandHandler(
            IValidator<BoardInitializeRequestDTO> validator,
            ILogger<BoardInitializeCommandHandler> logger,
            IBoardService service) : base(validator, logger, service)
        {
        }

        public async Task<IResponseTypes<BoardInitializeResponseDTO, ChessGameResponseMessage>> Handle(
            BoardInitializeCommand<IRequestTypes<BoardInitializeRequestDTO>,
                IResponseTypes<BoardInitializeResponseDTO, ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            var valisationResult = await _validator.ValidateAsync(request.RequestDTO.requestType, cancellationToken);

            if (!valisationResult.IsValid)
            {
                var errorMessages = valisationResult.Errors.Select(error => error.ErrorMessage).ToList();
                return ChessGameResponse<BoardInitializeResponseDTO>.
                    CreateErrorResponse(
                    ChessGameResponseMessage.GameCreationFailed,
                    HttpStatusCode.BadRequest, errorMessages);
            }

            var connectionRequestDto = new ConnectionRequestDTO<BoardInitializeRequestDTO>()
            {
                Data = new BoardInitializeRequestDTO()
                {
                    Player1Id = request.RequestDTO.requestType.Player1Id,
                    Player2Id = request.RequestDTO.requestType.Player2Id,
                }
            };
            var initializeGameResponseDTO = await _service.InitializeBoardAsync(connectionRequestDto);

            if (initializeGameResponseDTO.Data.GameId == default)
            {
                return ChessGameResponse<BoardInitializeResponseDTO>.
                    CreateErrorResponse(
                    ChessGameResponseMessage.GameCreationFailed,
                    HttpStatusCode.InternalServerError, new());
            }
            var BoardInitialize = new Board(default(FigureColors));

            var addingResult = ActiveGames.AddGame(initializeGameResponseDTO.Data.GameId, BoardInitialize);

            if (!addingResult)
            {
                _logger.LogError("Failed to add the new game with ID {GameId} to active games.", initializeGameResponseDTO.Data.GameId);
                return ChessGameResponse<BoardInitializeResponseDTO>.
                    CreateErrorResponse(
                    ChessGameResponseMessage.GameCreationFailed,
                    HttpStatusCode.InternalServerError, new());
            }

            var responseData = new BoardInitializeResponseDTO() { board = BoardInitialize, GameId = initializeGameResponseDTO.Data.GameId };


            return ChessGameResponse<BoardInitializeResponseDTO>.
                CreateSuccessResponse(
                responseData,
                ChessGameResponseMessage.GameCreated,
                HttpStatusCode.Created);
        }
    }
}
