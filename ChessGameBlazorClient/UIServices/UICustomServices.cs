using ChessGameBlazorClient.Contracts;
using ChessGameBlazorClient.ServiceEndpoints;
using ChessGameBlazorClient.UI.Services;

namespace WebAssemblyChessGame.UI.UIServices
{
    public static class UICustomServicesRegistration
    {
        public static void AddUICustomServices(this IServiceCollection services)
        {
            services.AddScoped<SignalRService>();
            services.AddScoped<ChessGameBlazorClient.ApiServices.IdentityService>();
            services.AddScoped<IQueryBuilder, QueryBuilder>();
        }
    }
}
