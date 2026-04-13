using ChessGame.Core.Services.PipeLine;
using ChessGame.Core.Services.PipeLine.Abstractions;
using Microsoft.Extensions.Logging;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

public class LoggingBehavior<TRequest, TResponse, TMessage>(
    ILogger<LoggingBehavior<TRequest, TResponse, TMessage>> logger)
    : IPipelineBehavior<TRequest, TResponse, TMessage>
    where TMessage : ChessGameResponseMessage
    where TRequest : RequestDTO
{
    public async Task<PipeLineResponse<TResponse, TMessage>> Handle(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse, TMessage>>> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {Request} (ConnectionId: {ConnectionId})",
            typeof(TRequest).Name,
            request.ConnectionId);

        var response = await next();

        logger.LogInformation("Handled {Request} (Success: {Success})",
            typeof(TRequest).Name,
            response.Response?.IsSuccess);

        return response;
    }
}