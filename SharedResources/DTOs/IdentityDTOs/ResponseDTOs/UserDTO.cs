using SharedResources.Contracts;
using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.IdentityDTOs.ResponseDTOs
{
    public class UserDTO : IIdentityResponseDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
