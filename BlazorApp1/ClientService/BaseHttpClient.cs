using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebAssemblyChessGame.UI.ClientService
{

    public class BaseHttpClient
    {
        protected readonly HttpClient _httpClient;
        public BaseHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "WebAssemblyChessGame.UI");
            _httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            _httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
        }
        protected async Task<T> PostAsync<T>(string url, T data)
        {
            try
            {
                var fullUrl = new Uri(_httpClient.BaseAddress, url).ToString();
                var response = await _httpClient.PostAsJsonAsync(url, data);

                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch (Exception ex)
            {
                var exMessage = ex.Message;
            }
            return default;
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}