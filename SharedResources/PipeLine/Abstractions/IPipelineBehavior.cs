using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;

namespace SharedResources.PipeLine.Abstractions;

public interface IPipelineBehavior<TRequest, TResponse> where TRequest : RequestDTO
{
    Task<PipeLineResponse<TResponse>> Handle(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse>>> next,
        CancellationToken cancellationToken);
}