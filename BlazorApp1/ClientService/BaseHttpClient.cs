using SharedResources.Contracts;
using SharedResources.Contracts.RequestsAndResponses;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebAssemblyChessGame.UI.Contracts;
using WebAssemblyChessGame.UI.ServiceEndpoints;
using static WebAssemblyChessGame.UI.ServiceEndpoints.Actions;
using static WebAssemblyChessGame.UI.ServiceEndpoints.Endpoints;

namespace WebAssemblyChessGame.UI.ClientService
{

    public class BaseHttpClient
    {
        protected IQueryBuilder _queryBuilder;
        protected readonly HttpClient _httpClient;
        public BaseHttpClient(HttpClient httpClient, IQueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
            _httpClient = httpClient;

            //_httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "WebAssemblyChessGame.UI");
            _httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            _httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
        }


        /// <summary>
        /// Sends a POST request with a typed request body and deserializes the typed response.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request body.</typeparam>
        /// <typeparam name="TResponse">The full response type implementing <see cref="IResponseTypes{TData, TMessage}"/>.</typeparam>
        /// <typeparam name="TData">The DTO type within the response.</typeparam>
        /// <typeparam name="TMessage">The message type used in the response.</typeparam>
        /// <param name="uri">The target URI of the POST request.</param>
        /// <param name="data">The request body to send.</param>
        /// <returns>A deserialized response of type <typeparamref name="TResponse"/>.</returns>
        protected async Task<TResponse?> PostAsync<TRequest, TResponse, TData, TMessage>(Uri uri, TRequest data)
        where TData : IResponseDTO
        where TMessage : IMessage
        where TResponse : IResponseTypes<TData, TMessage>
        {
            var response = await _httpClient.PostAsJsonAsync(uri, data);

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }


        /// <summary>
        /// Sends a GET request and deserializes the typed response.
        /// </summary>
        /// <typeparam name="TResponse">The full response type implementing <see cref="IResponseTypes{TData, TMessage}"/>.</typeparam>
        /// <param name="url">The full URL to send the GET request to.</param>
        /// <returns>A deserialized response of type <typeparamref name="TResponse"/>.</returns>
        protected async Task<TResponse?> GetAsync<TResponse>(string url)
        where TResponse : IResponseTypes<IResponseDTO, IMessage>
        {

            var response = await _httpClient.GetAsync(url);

            return await response.Content.ReadFromJsonAsync<TResponse>();

        }


        /// <summary>
        /// Builds a complete request URI for the specified Identity endpoint and action, 
        /// optionally appending query parameters.
        /// </summary>
        /// <param name="endpoint">The controller enum representing the Identity endpoint.</param>
        /// <param name="action">The action enum representing the specific API method.</param>
        /// <param name="queryParamAndValues">Optional list of query parameters to be appended to the URI.</param>
        /// <returns>A URI constructed from the base path and provided query parameters.</returns>
        protected Uri BuildRequestUri(IdentityEndpoints endpoint, IdentityAction action, List<KeyValuePair<string, string>> queryParamAndValues)
        {

            var identityBasePath = BasePaths.GetPath(endpoint, action);
            var requestQuery =
                 queryParamAndValues.Count > 0 ?
                 identityBasePath :
                 _queryBuilder.BuildPath(identityBasePath, queryParamAndValues);

            return requestQuery;
        }
        /// <summary>
        /// Builds a complete request URI for the specified Chat endpoint and action, 
        /// optionally appending query parameters.
        /// </summary>
        /// <param name="endpoint">The controller enum representing the Chat endpoint.</param>
        /// <param name="action">The action enum representing the specific API method.</param>
        /// <param name="queryParamAndValues">Optional list of query parameters to be appended to the URI.</param>
        /// <returns>A URI constructed from the base path and provided query parameters.</returns>

        protected Uri BuildRequestUri(ChatEndpoints endpoint, ChatAction action, List<KeyValuePair<string, string>> queryParamAndValues)
        {

            var identityBasePath = BasePaths.GetPath(endpoint, action);
            var requestQuery =
                 queryParamAndValues.Count > 0 ?
                 identityBasePath :
                 _queryBuilder.BuildPath(identityBasePath, queryParamAndValues);

            return requestQuery;
        }
        /// <summary>
        /// Builds a complete request URI for the specified ChessGame endpoint and action, 
        /// optionally appending query parameters.
        /// </summary>
        /// <param name="endpoint">The controller enum representing the ChessGame endpoint.</param>
        /// <param name="action">The action enum representing the specific API method.</param>
        /// <param name="queryParamAndValues">Optional list of query parameters to be appended to the URI.</param>
        /// <returns>A URI constructed from the base path and provided query parameters.</returns>

        protected Uri BuildRequestUri(ChessGameEndpoints endpoint, ChessGameAction action, List<KeyValuePair<string, string>> queryParamAndValues)
        {

            var identityBasePath = BasePaths.GetPath(endpoint, action);
            var requestQuery =
                 queryParamAndValues.Count > 0 ?
                 identityBasePath :
                 _queryBuilder.BuildPath(identityBasePath, queryParamAndValues);

            return requestQuery;
        }

    }
}