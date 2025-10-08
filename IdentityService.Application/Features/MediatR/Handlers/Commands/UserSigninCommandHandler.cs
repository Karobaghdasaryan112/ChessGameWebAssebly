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


        public async Task<IResponseTypes<SignInDTO, IdentityResponseMesage>> Handle(UserSigninCommand<IRequestTypes<LoginDTO>, IResponseTypes<SignInDTO, IdentityResponseMesage>> request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserSigninCommandHandler initialized.");
            var validationResult = await _validator.ValidateAsync(request._requestDTO.requestType, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return IdentityResponse<SignInDTO>.
                    _identityResponse.
                    CreateErrorResponse(
                    string.Join(", ", errors),
                    System.Net.HttpStatusCode.BadRequest);
            }

            var response = await _service.LoginAsync(request._requestDTO.requestType);

            if (response != null && response.IsSuccess)
            {
                return IdentityResponse<SignInDTO>.
                    _identityResponse.
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
                _identityResponse.
                CreateErrorResponse(
                IdentityResponseMesage.UserSignInFailed,
                System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
