using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? RefreshToken {  get; set; }
        public string? AccessToken {  get; set; }
        public DateTime RefreshTokenExpiryTime {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
