using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.MediatR.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionDTOs.GameRequestDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.ConnectionResponsDTOs.GameResponseDTOs;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.MediatR.Handlers.Queries
{

    public class GetMoveCommnadHandler :
        MediatR_Base<MoveRequestDTO, GetMoveCommnadHandler, IBoardService>,
        IRequestHandler<
            GetMoveCommand<MoveRequestDTO, ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>,
                ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>>
    {
        public GetMoveCommnadHandler(IValidator<MoveRequestDTO> validator, ILogger<GetMoveCommnadHandler> logger, IBoardService service)
            : base(validator, logger, service)
        {

        }

        public Task<ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> Handle(GetMoveCommand<MoveRequestDTO, ResponseDTO<MoveResponseDTO, ChessGameResponseMessage>> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
