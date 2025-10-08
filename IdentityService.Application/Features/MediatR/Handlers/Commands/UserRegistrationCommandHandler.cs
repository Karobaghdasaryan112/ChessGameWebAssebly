using FluentValidation;
using IdentityService.API.IdentityAPI.Contracts;
using IdentityService.API.IdentityAPI.Services;
using IdentityService.Application.Features.MediatR.Base;
using IdentityService.Application.Features.MediatR.Requests.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;
using SharedResources.DTOs.IdentityDTOs.ResponseDTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;

namespace IdentityService.Application.Features.MediatR.Handlers.Commands
{
    public class UserRegistrationCommandHandler :
         MediatR_Base<RegistrationDTO, UserRegistrationCommandHandler, IAuthService>,
         IRequestHandler<
             UserRegistrationCommand<
                 IRequestTypes<RegistrationDTO>, IResponseTypes<CreateUserDTO, IdentityResponseMesage>>,
                 IResponseTypes<CreateUserDTO, IdentityResponseMesage>>
    {
        public UserRegistrationCommandHandler(IValidator<RegistrationDTO> validator, ILogger<UserRegistrationCommandHandler> logger, IAuthService service)
            : base(validator, logger, service)
        {

        }

        public async Task<IResponseTypes<CreateUserDTO, IdentityResponseMesage>> Handle(UserRegistrationCommand<IRequestTypes<RegistrationDTO>, IResponseTypes<CreateUserDTO, IdentityResponseMesage>> request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(request.RequestDTO.requestType, cancellationToken);
            _logger.LogInformation("UserRegistrationCommandHandler initialized.");

            var response = await _service.CreateUserAsync(request.RequestDTO.requestType);
            if (response != null)
            {
                return
                    IdentityResponse<CreateUserDTO>.
                    _identityResponse.
                    CreateSuccessResponse(
                    new CreateUserDTO() { UserId = response.IsSuccess ? response.Data.UserId : 0 },
                    IdentityResponseMesage.UserCreated,
                    System.Net.HttpStatusCode.Created);
            }
            return
                IdentityResponse<CreateUserDTO>.
                _identityResponse.
                CreateErrorResponse(
                IdentityResponseMesage.UserCreationFailed,
                System.Net.HttpStatusCode.BadRequest);
        }
    }
}


