using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.Abstractions;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;

namespace SharedResources.PipeLine;

public class PipelineExecutor<TRequest, TResponse, TMessage>(
    IEnumerable<IPipelineBehavior<TRequest, TResponse, TMessage>> behaviors)
    : IPipelineExecutor<TRequest, TResponse, TMessage>
    where TMessage : ChessGameResponseMessage
    where TRequest : RequestDTO
{
    public async Task<PipeLineResponse<TResponse, TMessage>> Execute(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse, TMessage>>> handler,
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