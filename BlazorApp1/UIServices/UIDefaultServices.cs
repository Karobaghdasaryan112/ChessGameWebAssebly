using WebAssemblyChessGame.UI.ServiceEndpoints;

namespace WebAssemblyChessGame.UI.UIServices
{
    public static class UIDefaultServices
    {
        public static void AddUIDefaultServices(this IServiceCollection services)
        {
            services.AddScoped(sp => new HttpClient { });
            services.AddHttpClient("UIClient", client => client.BaseAddress = new Uri(BasePaths.baseUrl));
            services.AddAuthentication();
            services.AddAuthorizationCore();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          .AllowAnyOrigin();
                });
            });
        }
    }
}
