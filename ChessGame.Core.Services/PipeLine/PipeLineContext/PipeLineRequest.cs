namespace ChessGame.Core.Services.PipeLine;

public class PipeLineRequest<TRequest>(TRequest request, string connectionId)
{
    public TRequest Request { get; } = request;
    public string ConnectionId { get; } = connectionId;
}