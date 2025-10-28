using WebAssemblyChessGame.UI.ApiServices;
using WebAssemblyChessGame.UI.Contracts;
using WebAssemblyChessGame.UI.ServiceEndpoints;
using WebAssemblyChessGame.UI.Services;

namespace WebAssemblyChessGame.UI.UIServices
{
    public static class UICustomServicesRegistration
    {
        public static void AddUICustomServices(this IServiceCollection services)
        {
            services.AddScoped<SignalRService>();
            services.AddScoped<IdentityService>();
            services.AddScoped<IQueryBuilder, QueryBuilder>();

            services.AddAuthentication(option =>
            {
            });
        }
    }
}
