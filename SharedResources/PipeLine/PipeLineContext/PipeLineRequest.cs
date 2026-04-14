using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace SharedResources.PipeLine.PipeLineContext;

public class PipeLineRequest<TRequest>(TRequest request, string connectionId) where TRequest : RequestDTO
{
    public TRequest Request { get; } = request;
    public string ConnectionId { get; } = connectionId;
}