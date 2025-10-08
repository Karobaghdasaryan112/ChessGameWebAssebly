using SharedResources.DTOs.IdentityDTOs.RequestDTOs;
using WebAssemblyChessGame.UI.ClientService;

namespace WebAssemblyChessGame.UI.ApiServices
{
    public class IdentityService : BaseHttpClient
    {
        private const string IdentityPath = "api/Identity/";

        public IdentityService(HttpClient httpClient) : base(httpClient)
        {

        }

        public async Task<object> GetTokenAsync(LoginDTO loginRequest)
        {
            var url = $"{IdentityPath}login";

            return await PostAsync<LoginDTO>(url, loginRequest);
        }

        public async Task<bool> IsUserAuthenticated()
        {
            var url = $"{IdentityPath}is-authenticated";
            return await GetAsync<bool>(url);
        }
        public async Task<object> RegisterUserAsync(RegistrationDTO registerRequest)
        {
            var url = $"{IdentityPath}register";
            return await PostAsync<RegistrationDTO>(url, registerRequest);
        }
    }
}
