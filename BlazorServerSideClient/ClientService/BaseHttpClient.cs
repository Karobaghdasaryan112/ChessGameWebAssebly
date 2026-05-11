using ChessGameBlazorClient.Contracts;
using ChessGameBlazorClient.ServiceEndpoints;
using Newtonsoft.Json;
using SharedResources.Contracts;
using SharedResources.Contracts.RequestsAndResponses;
using System.Net.Http.Headers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static ChessGameBlazorClient.ServiceEndpoints.Actions;
using static ChessGameBlazorClient.ServiceEndpoints.Endpoints;
using SharedResources.DTOs.ChessGameDTOs.ResponseDTOs.MediatRResponseDTOs;
using SharedResources.Responses.ResponseMessages;

namespace ChessGameBlazorClient.UI.ClientService
{

    public class BaseHttpClient
    {
        protected IQueryBuilder _queryBuilder;
        protected readonly HttpClient _httpClient;
        protected readonly BasePaths _basePaths;
        public BaseHttpClient(HttpClient httpClient, IQueryBuilder queryBuilder,BasePaths basePaths)
        {
            _queryBuilder = queryBuilder;
            _httpClient = httpClient;
            _basePaths = basePaths;

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
        /// <typeparam name="TResponse">The full response type implementing <see cref="ResponseDTO{TData, TMessage}"/>.</typeparam>
        /// <typeparam name="TData">The DTO type within the response.</typeparam>
        /// <typeparam name="TMessage">The message type used in the response.</typeparam>
        /// <param name="uri">The target URI of the POST request.</param>
        /// <param name="data">The request body to send.</param>
        /// <returns>A deserialized response of type <typeparamref name="TResponse"/>.</returns>
        protected async Task<TResponse?> PostAsync<TRequest, TResponse, TData, TMessage>(Uri uri, TRequest data)
            where TResponse : ResponseDTO<TData, TMessage>
            where TData : IResponseDTO
            where TMessage : ChessGameResponseMessage
        {
            var response = await _httpClient.PostAsJsonAsync(uri, data);

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }


        /// <summary>
        /// Sends a GET request and deserializes the typed response.
        /// </summary>
        /// <typeparam name="TResponse">The full response type implementing <see cref="ResponseDTO{TData, TMessage}"/>.</typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="url">The full URL to send the GET request to.</param>
        /// <returns>A deserialized response of type <typeparamref name="TResponse"/>.</returns>
        protected async Task<TResponse?> GetAsync<TResponse, TData, TMessage>(string url)
            where TResponse : ResponseDTO<TData, TMessage>
            where TData : IResponseDTO
            where TMessage : ChessGameResponseMessage
        {
            try 
            {
                var response = await _httpClient.GetAsync(url);
        
                // This will throw a clear HttpRequestException if the status is 400, 500, etc.
                response.EnsureSuccessStatusCode(); 

                return await response.Content.ReadFromJsonAsync<TResponse>();
            }
            catch (HttpRequestException e)
            {
                // Log the status code and the URL to your console/output
                Console.WriteLine($"Request failed for {url}: {e.StatusCode}");
                throw; // Re-throw so your UI catch block can see it
            }
            catch (Exception e)
            {
                Console.WriteLine($"Critical error in GetAsync: {e.Message}");
                throw;
            }
        }


        /// <summary>
        /// Builds a complete request URI for the specified Identity endpoint and action, 
        /// optionally appending query parameters.
        /// </summary>
        /// <param name="endpoint">The controller enum representing the Identity endpoint.</param>
        /// <param name="action">The action enum representing the specific API method.</param>
        /// <param name="queryParamAndValues">Optional list of query parameters to be appended to the URI.</param>
        /// <returns>A URI constructed from the base path and provided query parameters.</returns>
        protected Uri BuildRequestUri(IdentityEndpoints endpoint, IdentityAction action,
            List<KeyValuePair<string, string>> queryParamAndValues)
        {

            var identityBasePath = _basePaths.GetPath(endpoint, action);
            var requestQuery =
                queryParamAndValues.Count > 0
                    ? identityBasePath
                    : _queryBuilder.BuildPath(identityBasePath, queryParamAndValues);

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

            var identityBasePath = _basePaths.GetPath(endpoint, action);
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
            try
            {
                var identityBasePath = _basePaths.GetPath(endpoint, action);
                var requestQuery =
                    queryParamAndValues.Count == 0
                        ? identityBasePath
                        : _queryBuilder.BuildPath(identityBasePath, queryParamAndValues);

                return requestQuery;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return default;
        }

    }
}