using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.IdentityDTOs.RequestDTOs
{
    public class LoginDTO : IIdentityRequestDTO
    {
        public string email { get; set; }
        public string password { get; set; }
        public int UserId { get ; set ; }
    }
}
