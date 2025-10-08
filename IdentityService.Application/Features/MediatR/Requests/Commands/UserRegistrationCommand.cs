using MediatR;
using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;
using SharedResources.DTOs.IdentityDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace IdentityService.Application.Features.MediatR.Requests.Commands
{
    public class UserRegistrationCommand<TRequest, TResponse> : IRequest<TResponse>
    {
        public TRequest RequestDTO { get; }

        public UserRegistrationCommand(TRequest request)
        {
            RequestDTO = request;
        }
    }

}

