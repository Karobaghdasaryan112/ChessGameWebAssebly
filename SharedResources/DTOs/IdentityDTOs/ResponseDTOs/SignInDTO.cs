using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.IdentityDTOs.ResponseDTOs
{
    public class SignInDTO : IIdentityResponseDTO
    {
        public string userName { get; set; }
        public string password { get; set; }
        public bool isPersistent { get; set; } = false;
        public bool lockoutOnFailure { get; set; } = false;
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        int IIdentityResponseDTO.UserId { get ; set ; }
    }
}
