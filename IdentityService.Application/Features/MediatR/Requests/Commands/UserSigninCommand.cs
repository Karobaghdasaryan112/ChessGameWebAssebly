using MediatR;

namespace IdentityService.Application.Features.MediatR.Requests.Commands
{
    public class UserSigninCommand<TRequest, TResponse> : IRequest<TResponse>
    {
        public TRequest _requestDTO { get; set; }
        public UserSigninCommand(TRequest requestDTO)
        {
            _requestDTO = requestDTO;
        }
    }
}
