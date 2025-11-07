using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace WebAssemblyChessGame.UI.UIServices
{
    public static class UIDefaultServices
    {
        public static void AddUIDefaultServices(this IServiceCollection services)
        {
            services.AddScoped(sp => new HttpClient { });

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
