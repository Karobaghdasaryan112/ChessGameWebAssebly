using ChessGame.Core.Services.PipeLine.Abstractions;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace ChessGame.Core.Services.PipeLine;

public class PipelineExecutor<TRequest, TResponse, TMessage>(IEnumerable<IPipelineBehavior<TRequest, TResponse, TMessage>> behaviors)
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

    public async Task<PipeLineResponse<TResponse, ChessGameResponseMessage>>
        Execute<TRequest, TResponse>(
            TRequest request,
            Func<Task<PipeLineResponse<TResponse, ChessGameResponseMessage>>> handler, HubCallerContext context)
        where TRequest : RequestDTO
    {
        var executor = context.GetHttpContext()!
            .RequestServices
            .GetRequiredService<
                IPipelineExecutor<TRequest, TResponse, ChessGameResponseMessage>>();

        return await executor.Execute(
            new PipeLineRequest<TRequest>(request, context.ConnectionId),
            handler,
            CancellationToken.None
        );
    }

}