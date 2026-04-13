using Microsoft.Extensions.Logging;

namespace ChessGame.Core.Services.PipeLine.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : Abstractions.IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<TRequest> _logger = logger;

    public Task<PipeLineResponse<TResponse>> Handle(PipeLineRequest<TRequest> request, Func<PipeLineRequest<TRequest>, Task<PipeLineResponse<TResponse>>> next)
    {
        _logger.LogInformation(request.ToString());
        return next(request);
    }
}