using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;

namespace SharedResources.PipeLine.Abstractions;

public interface IPipelineExecutor<TRequest, TResponse> where TRequest : RequestDTO
{
    Task<PipeLineResponse<TResponse>> Execute(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse>>> handler,
        CancellationToken cancellationToken);
}