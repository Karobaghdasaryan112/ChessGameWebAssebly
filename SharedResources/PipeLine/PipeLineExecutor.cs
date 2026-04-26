using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.Abstractions;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;

namespace SharedResources.PipeLine;

public class PipelineExecutor<TRequest, TResponse>(
    IEnumerable<IPipelineBehavior<TRequest, TResponse>> behaviors)
    : IPipelineExecutor<TRequest, TResponse>
    where TRequest : RequestDTO
{
    public async Task<PipeLineResponse<TResponse>> Execute(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse>>> handler,
        CancellationToken cancellationToken)
    {
        var pipeline = handler;

        foreach (var behavior in behaviors.Reverse())
        {
            var next = pipeline;
            pipeline = () => behavior.Handle(request, next, cancellationToken);
        }

        return await pipeline();
    }
}