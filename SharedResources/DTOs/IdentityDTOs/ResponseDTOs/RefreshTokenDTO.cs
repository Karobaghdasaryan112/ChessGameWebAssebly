using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.IdentityDTOs.ResponseDTOs
{
    public class RefreshTokenDTO : IIdentityResponseDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public int UserId { get; set; }
    }
}
