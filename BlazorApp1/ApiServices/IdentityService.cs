using SharedResources.Contracts.DTOs;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;
using SharedResources.DTOs.IdentityDTOs.ResponseDTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using WebAssemblyChessGame.UI.ClientService;
using WebAssemblyChessGame.UI.Contracts;
using static WebAssemblyChessGame.UI.ServiceEndpoints.Actions;
using static WebAssemblyChessGame.UI.ServiceEndpoints.Endpoints;

namespace WebAssemblyChessGame.UI.ApiServices
{
    public class IdentityService : BaseHttpClient
    {
        public IdentityService(HttpClient httpClient, IQueryBuilder queryBuilder) : base(httpClient, queryBuilder)
        {
        }


        /// <summary>
        /// Sends a login request to the identity API and returns a token response.
        /// </summary>
        /// <param name="loginRequest">The login data including email/username and password.</param>
        /// <param name="queryParamAndValues">Optional query parameters to include in the request URI.</param>
        /// <returns>A response containing the user data and token information.</returns>

        public async Task<IdentityResponse<UserDTO>?> GetTokenAsync(LoginDTO loginRequest, List<KeyValuePair<string, string>> queryParamAndValues)
        {
            var requestUri = this.BuildRequestUri(IdentityEndpoints.Identity, IdentityAction.Login, new List<KeyValuePair<string, string>>());

            return await PostAsync<LoginDTO,
                                   IdentityResponse<UserDTO>,
                                   UserDTO,
                                   IdentityResponseMesage>
                                   (requestUri, loginRequest);
        }


        /// <summary>
        /// Sends a user registration request to the identity API.
        /// </summary>
        /// <param name="registerRequest">The registration data including user details and password.</param>
        /// <param name="queryParamAndValues">Optional query parameters to include in the request URI.</param>
        /// <returns>A response containing the created user information.</returns>

        public async Task<IdentityResponse<CreateUserDTO>?> RegisterUserAsync(RegistrationDTO registerRequest, List<KeyValuePair<string, string>> queryParamAndValues)
        {
            var requestUri = this.BuildRequestUri(IdentityEndpoints.Identity, IdentityAction.Register, new List<KeyValuePair<string, string>>());

            return
                await PostAsync<RegistrationDTO,
                                   IdentityResponse<CreateUserDTO>,
                                   CreateUserDTO,
                                   IdentityResponseMesage>
                                   (requestUri, registerRequest);
        }

    }
}
