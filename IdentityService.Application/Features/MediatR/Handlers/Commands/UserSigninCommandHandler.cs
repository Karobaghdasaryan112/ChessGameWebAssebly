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
    /// Handles the user sign-in process using MediatR. 
    /// Validates the login request, delegates authentication to the <see cref="IAuthService"/>, 
    /// and returns a structured response based on the result.
    /// </summary>
    /// <remarks>
    /// Inherits common logic from <see cref="MediatR_Base{TDto, THandler, TService}"/>.
    /// Uses FluentValidation for input validation.
    /// </remarks>
    public class UserSigninCommandHandler :
        MediatR_Base<LoginDTO, UserSigninCommandHandler, IAuthService>,
        IRequestHandler<
            UserSigninCommand<
                IRequestTypes<LoginDTO>, IResponseTypes<SignInDTO, IdentityResponseMesage>>,
                IResponseTypes<SignInDTO, IdentityResponseMesage>>

    {
        public UserSigninCommandHandler(IValidator<LoginDTO> validator, ILogger<UserSigninCommandHandler> logger, IAuthService service) : base(validator, logger, service)
        {

        }
        /// <summary>
        /// Processes a <see cref="UserSigninCommand{TRequest, TResponse}"/> by validating the login data, 
        /// performing authentication via the <see cref="IAuthService"/>, and returning a standardized response.
        /// </summary>
        /// <param name="request">
        /// The sign-in command request containing the login data wrapped in a request DTO.
        /// </param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// A task that resolves to an <see cref="IResponseTypes{SignInDTO, IdentityResponseMesage}"/> containing either:
        /// - Success: a <see cref="SignInDTO"/> with token and user data, or
        /// - Failure: validation or authentication error details.
        /// </returns>


        public async Task<IResponseTypes<SignInDTO, IdentityResponseMesage>> Handle(UserSigninCommand<IRequestTypes<LoginDTO>, IResponseTypes<SignInDTO, IdentityResponseMesage>> request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request._requestDTO.requestType, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return IdentityResponse<SignInDTO>.
                    CreateErrorResponse(
                    IdentityResponseMesage.Initialize,
                    System.Net.HttpStatusCode.BadRequest,
                    errors);
            }

            var response = await _service.LoginAsync(request._requestDTO.requestType);

            if (response != null && response.IsSuccess)
            {
                return IdentityResponse<SignInDTO>.
                    CreateSuccessResponse(
                    new SignInDTO()
                    {
                        AccessToken = response.Data.AccessToken,
                        RefreshToken = response.Data.RefreshToken,
                        userName = response.Data.userName,
                        UserId = response.Data.UserId,
                    },
                    IdentityResponseMesage.UserSignedIn,
                    System.Net.HttpStatusCode.OK);
            }
            return IdentityResponse<SignInDTO>.
                CreateErrorResponse(
                IdentityResponseMesage.UserSignInFailed,
                System.Net.HttpStatusCode.Unauthorized,
                new List<string>());
        }
    }
}
