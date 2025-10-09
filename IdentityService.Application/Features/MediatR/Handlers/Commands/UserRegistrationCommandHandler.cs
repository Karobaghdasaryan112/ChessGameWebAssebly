using FluentValidation;
using IdentityService.API.IdentityAPI.Contracts;
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
    /// <summary>
    /// Handles the user registration logic using MediatR by validating the input,
    /// delegating user creation to the authentication service, and returning a standardized response.
    /// </summary>
    /// <remarks>
    /// Inherits shared logic such as validation and logging from <see cref="MediatR_Base{TValidationType, TloggerType, TService}"/>.
    /// </remarks>

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
        /// <summary>
        /// Processes a <see cref="UserRegistrationCommand{TRequest, TResponse}"/> by validating the request,
        /// calling the user creation service, and returning a response wrapped in <see cref="IResponseTypes{TData, TMessage}"/>.
        /// </summary>
        /// <param name="request">The registration command containing the user's registration data.</param>
        /// <param name="cancellationToken">A cancellation token for cooperative task cancellation.</param>
        /// <returns>
        /// A success response with created user data if registration succeeds,
        /// or an error response if it fails.
        /// </returns>

        public async Task<IResponseTypes<CreateUserDTO, IdentityResponseMesage>> Handle(
            UserRegistrationCommand<
                IRequestTypes<RegistrationDTO>,
                IResponseTypes<CreateUserDTO, IdentityResponseMesage>> request,
                CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(request.RequestDTO.requestType, cancellationToken);

            var response = await _service.CreateUserAsync(request.RequestDTO.requestType);
            if (response != null && response.IsSuccess)
            {
                return
                    IdentityResponse<CreateUserDTO>
                    .CreateSuccessResponse(
                    new CreateUserDTO() { UserId = response.Data.UserId },
                    IdentityResponseMesage.UserCreated,
                    System.Net.HttpStatusCode.Created);
            }
            return
                IdentityResponse<CreateUserDTO>
                .CreateErrorResponse(
                IdentityResponseMesage.UserCreationFailed,
                System.Net.HttpStatusCode.BadRequest,
                new List<string>());
        }
    }
}


