using Microsoft.Extensions.Logging;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.Abstractions;
using SharedResources.PipeLine.PipeLineContext;

namespace SharedResources.PipeLine.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : RequestDTO
{
    public async Task<PipeLineResponse<TResponse>> Handle(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse>>> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {Request}",
            typeof(TRequest).Name);

        var response = await next();

        logger.LogInformation("Handled {Request} (Success: {Success})",
            typeof(TRequest).Name,
            response.Response?.IsSuccess);

        return response;
    }
}