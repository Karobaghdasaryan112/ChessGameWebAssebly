using ChessGame.Core.Services.PipeLine.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Internal;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.PipeLine.PipeLineHelper
{
    public class PipeLineExecutionHelper
    {
        public async Task<PipeLineResponse<TResponse, ChessGameResponseMessage>>
            Execute<TRequest, TResponse>(
                TRequest request,
                HubCallerContext context,
                Func<Task<PipeLineResponse<TResponse, ChessGameResponseMessage>>> handler)
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
}
