using FluentValidation;
using IdentityService.API.IdentityAPI.Contracts;
using IdentityService.Application.Features.MediatR.Requests.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;
using SharedResources.DTOs.IdentityDTOs.ResponseDTOs;
using SharedResources.MediatR;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using System.Net;

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
                 IRequestTypes<RegistrationDTO>, IResponseTypes<RegistrationResponseDTO, IdentityResponseMesage>>,
                 IResponseTypes<RegistrationResponseDTO, IdentityResponseMesage>>
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

        public async Task<IResponseTypes<RegistrationResponseDTO, IdentityResponseMesage>> Handle(
            UserRegistrationCommand<
                IRequestTypes<RegistrationDTO>,
                IResponseTypes<RegistrationResponseDTO, IdentityResponseMesage>> request,
                CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.RequestDTO.requestType, cancellationToken);
            if (!validationResult.IsValid)
                return IdentityResponse<RegistrationResponseDTO>.
                    CreateErrorResponse(
                        IdentityResponseMesage.UserCreationFailed,
                        System.Net.HttpStatusCode.BadRequest,
                        validationResult.Errors.Select(error => error.ErrorMessage).ToList());

            try
            {
                return await _service.CreateUserAsync(request.RequestDTO.requestType, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration.");
                return IdentityResponse<RegistrationResponseDTO>.CreateErrorResponse(
                    IdentityResponseMesage.UserCreationFailed,
                    System.Net.HttpStatusCode.InternalServerError,
                    new List<string>());
            }
        }
    }
}


