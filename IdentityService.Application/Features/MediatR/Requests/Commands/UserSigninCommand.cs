using MediatR;
using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.IdentityDTOs;
using SharedResources.Responses.ResponseMessages;

namespace IdentityService.Application.Features.MediatR.Requests.Commands
{
    public class UserSigninCommand<TRequest,TResponse> : IRequest<IResponseTypes<TResponse, IdentityResponseMesage>> 
        where TResponse : class, IIdentityDTO
        where TRequest : class, IIdentityDTO
    {
        public TRequest _request { get; set; }
        public TResponse _response { get; set; }
        public UserSigninCommand(TRequest requestDTO)
        {
            _request = requestDTO;
        }
    }
}
