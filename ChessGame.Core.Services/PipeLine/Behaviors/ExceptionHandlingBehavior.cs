using MediatR;

namespace ChessGame.Core.Services.PipeLine.Behaviors;

public class ExceptionHandlingBehavior<TRequest, TResponse> : Abstractions.IPipelineBehavior<TRequest, TResponse>
{
    public async Task<PipeLineResponse<TResponse>> Handle(
        PipeLineRequest<TRequest> request, 
        Func<PipeLineRequest<TRequest>, Task<PipeLineResponse<TResponse>>> next)
    {
        try
        {
            return await next(request);
        }
        catch (Exception ex)
        {
            return new PipeLineResponse<TResponse>(default, false,ex.Message);
        }
    }
}