using Microsoft.AspNetCore.Identity;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.IdentityDTOs;
using SharedResources.Responses.ResponseMessages;

namespace IdentityService.API.IdentityAPI.Contracts
{
    public interface IAuthService
    {
        Task<SignInResult> PasswordSignInAsync(SignInDTO signInDTO);
        Task<IResponseTypes<CreateUserDTO, IdentityResponseMesage>> CreateUserAsync(RegistrationDTO registrationDTO);
        Task<IResponseTypes<SignInDTO, IdentityResponseMesage>> LoginAsync(LoginDTO loginDTO);
        Task SignOutAsync();
    }
}
