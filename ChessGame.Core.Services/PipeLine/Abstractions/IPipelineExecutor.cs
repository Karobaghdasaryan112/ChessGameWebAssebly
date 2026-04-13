using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGame.Core.Services.PipeLine.Abstractions;

public interface IPipelineExecutor<TRequest, TResponse,TMessage> where TMessage : ChessGameResponseMessage where TRequest : RequestDTO
 {
    Task<PipeLineResponse<TResponse,TMessage>> Execute(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse, TMessage>>> handler,
        CancellationToken cancellationToken);
}