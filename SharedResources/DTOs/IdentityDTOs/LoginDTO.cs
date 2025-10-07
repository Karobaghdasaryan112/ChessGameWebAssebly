using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.IdentityDTOs
{
    public class LoginDTO : IIdentityDTO
    {
        public string email { get; set; }
        public string password { get; set; }
        public int UserId { get ; set ; }
    }
}
