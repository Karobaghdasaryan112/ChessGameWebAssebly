using System.Net;
using ChessGame.Core.Services.Services.Validations;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.PipeLine.Abstractions;
using SharedResources.PipeLine.PipeLineContext;
using SharedResources.Responses.ResponseMessages;

namespace SharedResources.PipeLine.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(
    GenericValidationService validation)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : RequestDTO
{
    public async Task<PipeLineResponse<TResponse>> Handle(PipeLineRequest<TRequest> request,
        Func<Task<PipeLineResponse<TResponse>>> next, CancellationToken cancellationToken)
    {
        var validationResult = await validation.ValidateAsync(request.Request);

        if (validationResult.IsValid)
            return await next();

        return new PipeLineResponse<TResponse>
        {
            Response = ResponseDTO<TResponse,ChessGameResponseMessage>.CreateErrorResponse(
                default!,
                (ChessGameResponseMessage.InvalidData),
                HttpStatusCode.BadRequest,
                validationResult.Errors?.Select(e => e.ErrorMessage).ToList())
        };
    }


}