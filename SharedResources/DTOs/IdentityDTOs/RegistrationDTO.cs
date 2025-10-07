using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.IdentityDTOs
{
    public class RegistrationDTO : IIdentityDTO
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }
        public int UserId { get; set; }
    }
}
