using ChessGame.Core.Services.PipeLine.Abstractions;

namespace ChessGame.Core.Services.PipeLine;

public class PipeLineExecutor<TRequest, TResponse> : IPipelineExecutor<TRequest, TResponse>
{
    private readonly List<Func<
        Func<PipeLineRequest<TRequest>, Task<PipeLineResponse<TResponse>>>,
        Func<PipeLineRequest<TRequest>, Task<PipeLineResponse<TResponse>>>
    >> _components = new();


    private void Use(Func<
        Func<PipeLineRequest<TRequest>, Task<PipeLineResponse<TResponse>>>,
        Func<PipeLineRequest<TRequest>, Task<PipeLineResponse<TResponse>>>> middleware)
    {
        _components.Add(middleware);
    }

    
    
    public PipeLineExecutor<TRequest, TResponse> UseBehavior(IPipelineBehavior<TRequest, TResponse> behavior)
    {
        Use(next => request => behavior.Handle(request, next));
        return this;
    }


    public async Task<PipeLineResponse<TResponse>> Execute(
        PipeLineRequest<TRequest> request,
        Func<PipeLineRequest<TRequest>, Task<PipeLineResponse<TResponse>>> executionalFunction,
        CancellationToken cancellationToken)
    {

        var chain = executionalFunction;

        for (var i = _components.Count - 1; i >= 0; i--)
        {
            var component = _components[i];
            chain = component(chain);
        }
        
        return await chain(request);
    }
}