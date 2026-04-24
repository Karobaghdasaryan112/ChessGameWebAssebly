using Microsoft.Extensions.Logging;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.Abstractions;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;

namespace SharedResources.PipeLine.Behaviors;

public class ExceptionHandlingBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : RequestDTO
{
    public async Task<PipeLineResponse<TResponse>> Handle(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse>>> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in {Request}", typeof(TRequest).Name);



            return new PipeLineResponse<TResponse>
            {
                Response = new ResponseDTO<TResponse,ChessGameResponseMessage>
                {
                    IsSuccess = false,
                    Errors = [ex.Message],
                    Message = ChessGameResponseMessage.InternalServerError
                }
            };
        }
    }
}