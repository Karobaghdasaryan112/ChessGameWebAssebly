using Microsoft.Extensions.Logging;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.Abstractions;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;

namespace SharedResources.PipeLine.Behaviors;

public class ExceptionHandlingBehavior<TRequest, TResponse, TMessage>(
    ILogger<ExceptionHandlingBehavior<TRequest, TResponse, TMessage>> logger)
    : IPipelineBehavior<TRequest, TResponse, TMessage>
    where TMessage : ChessGameResponseMessage
    where TRequest : RequestDTO
{
    public async Task<PipeLineResponse<TResponse, TMessage>> Handle(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse, TMessage>>> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in {Request}", typeof(TRequest).Name);

            return new PipeLineResponse<TResponse, TMessage>
            {
                Response = new ResponseDTO<TResponse, TMessage>
                {
                    IsSuccess = false,
                    Errors = [ex.Message],
                    Message = (TMessage)Activator.CreateInstance(typeof(TMessage))!
                }
            };
        }
    }
}