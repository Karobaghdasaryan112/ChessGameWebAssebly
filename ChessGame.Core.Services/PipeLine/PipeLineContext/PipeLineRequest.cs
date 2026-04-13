using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;

namespace ChessGame.Core.Services.PipeLine;

public class PipeLineRequest<TRequest>(TRequest request, string connectionId) where TRequest : RequestDTO
{
    public TRequest Request { get; } = request;
    public string ConnectionId { get; } = connectionId;
}