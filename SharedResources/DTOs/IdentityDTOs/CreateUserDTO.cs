using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.IdentityDTOs
{
    public class CreateUserDTO : IIdentityDTO
    {
        public int UserId { get; set; }
        int IIdentityDTO.UserId { get; set; }
    }
}
