using System.Net;
using ChessGame.Core.Services.Services.Validations;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.Abstractions;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;

namespace SharedResources.PipeLine.Behaviors;

public class ValidationBehavior<TRequest, TResponse, TMessage>(
    GenericValidationService validation)
    : IPipelineBehavior<TRequest, TResponse, TMessage>
    where TRequest : RequestDTO
    where TMessage : ChessGameResponseMessage
{
    public async Task<PipeLineResponse<TResponse, TMessage>> Handle(
        PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse, TMessage>>> next,
        CancellationToken cancellationToken)
    {
        var validationResult = await validation.ValidateAsync(request.Request);

        if (validationResult.IsValid)
            return await next();

        return new PipeLineResponse<TResponse, TMessage>
        {
            Response = ResponseDTO<TResponse, TMessage>.CreateErrorResponse(
                default!,
                (TMessage)ChessGameResponseMessage.InvalidData,
                HttpStatusCode.BadRequest,
                validationResult.Errors.Select(e => e.ErrorMessage).ToList())
        };
    }
}