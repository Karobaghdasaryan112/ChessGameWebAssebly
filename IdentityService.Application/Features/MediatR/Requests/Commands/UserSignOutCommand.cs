using MediatR;

namespace IdentityService.Application.Features.MediatR.Requests.Commands
{
    public class UserSignOutCommand<TRequest, TResponse> : IRequest<TResponse>
    {
    }
}
