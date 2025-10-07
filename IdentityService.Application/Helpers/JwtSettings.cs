namespace IdentityService.API.IdentityAPI.Helpers
{
    public class JwtSettings
    {
        public string SecurityKey { get; set; }
        public int ExpirationInMinutes { get; set; }
        public int RefreshTokenExpirationInDays { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public bool SlidingExpiration { get; set; }
    }
}
