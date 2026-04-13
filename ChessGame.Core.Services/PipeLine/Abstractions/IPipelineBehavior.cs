namespace ChessGame.Core.Services.PipeLine.Abstractions;

public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<PipeLineResponse<TResponse>> Handle(
        PipeLineRequest<TRequest> request,
        Func<PipeLineRequest<TRequest>, Task<PipeLineResponse<TResponse>>> next);
}