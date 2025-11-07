using Microsoft.Extensions.DependencyInjection;
using ServerSideClientUI.Contracts;
using ServerSideClientUI.ServiceEndpoints;
using WebAssemblyChessGame.UI.Services;

namespace WebAssemblyChessGame.UI.UIServices
{
    public static class UICustomServicesRegistration
    {
        public static void AddUICustomServices(this IServiceCollection services)
        {
            services.AddScoped<SignalRService>();
            services.AddScoped<ServerSideClientUI.ApiServices.IdentityService>();
            services.AddScoped<IQueryBuilder, QueryBuilder>();
        }
    }
}
