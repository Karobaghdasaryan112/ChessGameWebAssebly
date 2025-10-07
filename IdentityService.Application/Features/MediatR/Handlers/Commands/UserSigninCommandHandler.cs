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
    public class UserSigninCommandHandler :
        MediatR_Base<LoginDTO, UserSigninCommandHandler, AuthService, LoginDTO>,
        IRequestHandler<UserSigninCommand<LoginDTO, SignInDTO>, IResponseTypes<SignInDTO, IdentityResponseMesage>>
    {
        public UserSigninCommandHandler(IValidator<LoginDTO> validator, ILogger<UserSigninCommandHandler> logger, AuthService service) : base(validator, logger, service)
        {

        }

        public async Task<IResponseTypes<SignInDTO, IdentityResponseMesage>> Handle(UserSigninCommand<LoginDTO, SignInDTO> request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request._request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return await IdentityResponse<SignInDTO>.
                    identityResponse.
                    CreateErrorResponse(
                    string.Join(", ", errors),
                    System.Net.HttpStatusCode.BadRequest);
            }

            _logger.LogInformation("UserSigninCommandHandler initialized.");

            var response = await _service.LoginAsync(request._request);

            if (response != null && response.IsSuccess)
            {
                return await IdentityResponse<SignInDTO>.
                    identityResponse.
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
            return await IdentityResponse<SignInDTO>.
                identityResponse.
                CreateErrorResponse(
                IdentityResponseMesage.UserSignInFailed,
                System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
