using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using ChessGame.Core.Services.Services.Board;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.ChessGameResource.Models;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
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

        public BoardInitializeCommandHandler(IValidator<BoardInitializeRequestDTO> validator, ILogger<BoardInitializeCommandHandler> logger, BoardService service) : base(validator, logger, service)
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

            var BoardInitialize = new Board(request.RequestDTO.requestType.MyFigureColor);

            var responseData = new BoardInitializeResponseDTO() { board = BoardInitialize };

            return ChessGameResponse<BoardInitializeResponseDTO>.
                CreateSuccessResponse(
                responseData,
                ChessGameResponseMessage.GameCreated,
                HttpStatusCode.Created);
        }
    }
}
