using FluentValidation;
using IdentityService.API.IdentityAPI.Services;
using IdentityService.Application.Features.MediatR.Base;
using IdentityService.Application.Features.MediatR.Requests.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.IdentityDTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;

namespace IdentityService.Application.Features.MediatR.Handlers.Commands
{
    public class UserRegistrationCommandHandler :
        MediatR_Base<RegistrationDTO, UserRegistrationCommandHandler, AuthService, RegistrationDTO>,
        IRequestHandler<UserRegistrationCommand<RegistrationDTO>, IResponseTypes<RegistrationDTO, IdentityResponseMesage>>
    {
        public UserRegistrationCommandHandler(IValidator<RegistrationDTO> validator, ILogger<UserRegistrationCommandHandler> logger, AuthService service)
            : base(validator, logger, service)
        {
        }

        public async Task<IResponseTypes<RegistrationDTO, IdentityResponseMesage>> Handle(UserRegistrationCommand<RegistrationDTO> request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(request.UserForRegistrationDTO, cancellationToken);
            _logger.LogInformation("UserRegistrationCommandHandler initialized.");

            var response = await _service.CreateUserAsync(request.UserForRegistrationDTO);
            if (response != null)
            {
                return await IdentityResponse<RegistrationDTO>.
                    identityResponse.
                    CreateSuccessResponse(
                    new RegistrationDTO() { UserId = response.Data.UserId },
                    IdentityResponseMesage.UserCreated,
                    System.Net.HttpStatusCode.Created);
            }
            return await IdentityResponse<RegistrationDTO>.
                identityResponse.
                CreateErrorResponse(
                IdentityResponseMesage.UserCreationFailed,
                System.Net.HttpStatusCode.BadRequest);
        }
    }
}


