using SharedResources.Contracts.DTOs;

namespace SharedResources.DTOs.IdentityDTOs.ResponseDTOs
{
    public class CreateUserDTO : IIdentityResponseDTO
    {
        public int UserId { get; set; }
    }
}
