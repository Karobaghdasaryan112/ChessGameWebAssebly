using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.Abstractions;
using SharedResources.PipeLine.PipeLineContext;

namespace SharedResources.PipeLine.PipeLineHelper
{
    public class PipeLineExecutionHelper
    {
        public async Task<PipeLineResponse<TResponse>>
            Execute<TRequest, TResponse>(TRequest request, HubCallerContext context,
                Func<Task<PipeLineResponse<TResponse>>> handler)
            where TRequest : RequestDTO
        {
            var executor = context.GetHttpContext()!
                .RequestServices
                .GetRequiredService<
                    IPipelineExecutor<TRequest, TResponse>>();

            request.connectionId = context.ConnectionId;
            return await executor.Execute(
                new PipeLineRequest<TRequest>(request),
                handler,
                CancellationToken.None
            );
        }
    }
}