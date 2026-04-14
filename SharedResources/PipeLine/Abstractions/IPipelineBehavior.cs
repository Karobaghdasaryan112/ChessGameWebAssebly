using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;

namespace SharedResources.PipeLine.Abstractions;

public interface IPipelineBehavior<TRequest, TResponse,TMessage> where TMessage : ChessGameResponseMessage where TRequest : RequestDTO
{
    Task<PipeLineResponse<TResponse,TMessage>> Handle(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse,TMessage>>> next,
        CancellationToken cancellationToken);
}