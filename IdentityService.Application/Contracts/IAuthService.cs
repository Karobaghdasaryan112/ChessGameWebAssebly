using Microsoft.AspNetCore.Identity;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;
using SharedResources.DTOs.IdentityDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace IdentityService.API.IdentityAPI.Contracts
{
    public interface IAuthService
    {
        Task<SignInResult> PasswordSignInAsync(SignInDTO signInDTO);
        Task<IResponseTypes<RegistrationResponseDTO, IdentityResponseMesage>> CreateUserAsync(RegistrationDTO registrationDTO, CancellationToken cancellationToken);
        Task<IResponseTypes<SignInDTO, IdentityResponseMesage>> LoginAsync(LoginDTO loginDTO);
        Task SignOutAsync();
    }
}
