using ChessGameBlazorClient.Contracts;
using ChessGameBlazorClient.UI.ClientService;
using SharedResources.DTOs.IdentityDTOs.RequestDTOs;
using SharedResources.DTOs.IdentityDTOs.ResponseDTOs;
using SharedResources.Responses;
using SharedResources.Responses.ResponseMessages;
using static ChessGameBlazorClient.ServiceEndpoints.Actions;
using static ChessGameBlazorClient.ServiceEndpoints.Endpoints;

namespace ChessGameBlazorClient.ApiServices
{
    public class IdentityService(HttpClient httpClient, IQueryBuilder queryBuilder)
        : BaseHttpClient(httpClient, queryBuilder)
    {
        /// <summary>
        /// Sends a login request to the identity API and returns a token response.
        /// </summary>
        /// <param name="loginRequest">The login data including email/username and password.</param>
        /// <param name="queryParamAndValues">Optional query parameters to include in the request URI.</param>
        /// <returns>A response containing the user data and token information.</returns>

        public async Task<IdentityResponse<UserDTO>?> GetTokenAsync(LoginDTO loginRequest, List<KeyValuePair<string, string>> queryParamAndValues)
        {
            var requestUri = this.BuildRequestUri(IdentityEndpoints.Identity, IdentityAction.Login, []);

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

        public async Task<IdentityResponse<RegistrationResponseDTO>?> RegisterUserAsync(RegistrationDTO registerRequest, List<KeyValuePair<string, string>> queryParamAndValues)
        {
            var requestUri = this.BuildRequestUri(IdentityEndpoints.Identity, IdentityAction.Register, new());

            return
                await PostAsync<RegistrationDTO,
                                   IdentityResponse<RegistrationResponseDTO>,
                                   RegistrationResponseDTO,
                                   IdentityResponseMesage>
                                   (requestUri, registerRequest);
        }

        public async Task<IdentityResponse<UserDTO>?> GetUsersByIdsAsync(List<string> Ids, List<KeyValuePair<string, string>> queryParamAndValues)
        {
            var requestUri = this.BuildRequestUri(IdentityEndpoints.Identity, IdentityAction.GetUsersByIds, new());

            return
                await PostAsync<List<string>,
                                   IdentityResponse<UserDTO>,
                                   UserDTO,
                                   IdentityResponseMesage>
                                   (requestUri, Ids);
        }

        public async Task<IdentityResponse<SignInDTO>?> LoginUserAsync(LoginDTO registerRequest, List<KeyValuePair<string, string>> queryParamAndValues)
        {
            var requestUri = this.BuildRequestUri(IdentityEndpoints.Identity, IdentityAction.Login, new());

            var result
                = await PostAsync<LoginDTO,
                                   IdentityResponse<SignInDTO>,
                                   SignInDTO,
                                   IdentityResponseMesage>
                                   (requestUri, registerRequest);

            return result;
        }
    }
}
