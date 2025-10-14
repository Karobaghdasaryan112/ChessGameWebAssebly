using ChessGame.Core.Services.MediatR.Base;
using ChessGame.Core.Services.MediatR.Requests.Commands;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.MediatR.Handlers.Commands
{
    public class SubmitMoveCommandHandler :
        MediatR_Base<SubmitMoveRequestDTO, SubmitMoveCommandHandler, bool>,
        IRequestHandler<
            SubmitMoveCommand<
                IRequestTypes<SubmitMoveRequestDTO>, IResponseTypes<SubmitMoveResponseDTO, ChessGameResponseMessage>>,
            IResponseTypes<SubmitMoveResponseDTO, ChessGameResponseMessage>>
    {
        public SubmitMoveCommandHandler(IValidator<SubmitMoveRequestDTO> validator, ILogger<SubmitMoveCommandHandler> logger, bool service) : base(validator, logger, service)
        {
        }

        public Task<IResponseTypes<SubmitMoveResponseDTO, ChessGameResponseMessage>> Handle(
            SubmitMoveCommand<IRequestTypes<SubmitMoveRequestDTO>,
            IResponseTypes<SubmitMoveResponseDTO,
            ChessGameResponseMessage>> request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
