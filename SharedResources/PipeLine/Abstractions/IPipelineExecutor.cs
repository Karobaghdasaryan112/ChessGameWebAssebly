using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;

namespace SharedResources.PipeLine.Abstractions;

public interface IPipelineExecutor<TRequest, TResponse,TMessage> where TMessage : ChessGameResponseMessage where TRequest : RequestDTO
 {
    Task<PipeLineResponse<TResponse,TMessage>> Execute(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse, TMessage>>> handler,
        CancellationToken cancellationToken);
}