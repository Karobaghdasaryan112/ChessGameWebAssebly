using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.PipeLine.Abstractions;

public interface IPipelineBehavior<TRequest, TResponse,TMessage> where TMessage : ChessGameResponseMessage where TRequest : RequestDTO
{
    Task<PipeLineResponse<TResponse,TMessage>> Handle(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse,TMessage>>> next,
        CancellationToken cancellationToken);
}