using System.Net;
using WebAssemblyChessGame.UI.ServiceEndpoints;

namespace WebAssemblyChessGame.UI.UIServices
{
    public static class UIDefaultServices
    {
        public static void AddUIDefaultServices(this IServiceCollection services)
        {
            services.AddScoped(sp => new HttpClient { });
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

            services.AddHttpClient("IdentityAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7225"); 
            })
             .ConfigurePrimaryHttpMessageHandler(() =>
             {
                 return new HttpClientHandler
                 {
                     UseCookies = true,
                     CookieContainer = new CookieContainer(), 
                     AllowAutoRedirect = false
                 };
             });

        }
    }
}
