using MediatR;
using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.Responses.ResponseMessages;

namespace IdentityService.Application.Features.MediatR.Requests.Commands
{
    public class UserSignOutCommand<TDto> : IRequest<IResponseTypes<TDto, IdentityResponseMesage>> where TDto : class, IIdentityDTO
    {
    }
}
