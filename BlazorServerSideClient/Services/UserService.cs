using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlazorServerSideClient.Services
{
    public class UserService
    {
        internal static AuthenticationStateProvider _authStateProvider;
        private UserManager<IdentityUser> _userManager { get; set; }

        public async Task<ClaimsPrincipal> GetCurrentUserAsync()
        {
            if (_authStateProvider == null)
                throw new InvalidOperationException("AuthenticationStateProvider is not initialized.");

            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User;
        }
        public async Task<IEnumerable<Claim>> GetCurrentUserInfo()
        {
            if (_authStateProvider == null)
                throw new InvalidOperationException("AuthenticationStateProvider is not initialized.");

            var user = await GetCurrentUserAsync();
            return user.Claims;
        }
    }
}
