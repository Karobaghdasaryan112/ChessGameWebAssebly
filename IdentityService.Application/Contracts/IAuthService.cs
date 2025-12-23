using Microsoft.AspNetCore.Identity;
using SharedResources.Contracts.RequestsAndResponses;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;
using SharedResources.DTOs.IdentityDTOs.ResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace IdentityService.API.IdentityAPI.Contracts
{
    public interface IAuthService
    {
        Task<SignInResult> PasswordSignInAsync(SignInDTO signInDTO);
        Task<ResponseDTO<RegistrationResponseDTO, IdentityResponseMesage>> CreateUserAsync(RegistrationDTO registrationDTO, CancellationToken cancellationToken);
        Task<ResponseDTO<SignInDTO, IdentityResponseMesage>> LoginAsync(LoginDTO loginDTO);
        Task SignOutAsync();
    }
}
