using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.IdentityDTOs.ResponseDTOs
{
    public class RegistrationResponseDTO : IIdentityResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        int IIdentityResponseDTO.UserId { get; set; }
    }
}
