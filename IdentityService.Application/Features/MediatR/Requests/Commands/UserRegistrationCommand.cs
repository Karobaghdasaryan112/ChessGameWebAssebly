using MediatR;
using SharedResources.Contracts.DTOs;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.IdentityDTOs;
using SharedResources.Responses.ResponseMessages;

namespace IdentityService.Application.Features.MediatR.Requests.Commands
{
    public class UserRegistrationCommand<TDto> : IRequest<IResponseTypes<TDto, IdentityResponseMesage>> where TDto : class, IIdentityDTO
    {
        public TDto UserForRegistrationDTO { get; set; }
        public UserRegistrationCommand(TDto userForRegistrationDTO)
        {
            UserForRegistrationDTO = userForRegistrationDTO;
        }
    }
}
