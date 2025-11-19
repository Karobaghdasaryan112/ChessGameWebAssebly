using ChessGame.Core.Services.Services.HubServices;
using ChessGame.Infrastructure.Infrastructure.Hubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChessGame.Infrastructure.Infrastructure
{
    public static class InfrastructureServices
    {
        public static void AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {

            services.AddScoped<GameHub>();
            services.AddScoped<BaseHubService<GameHub>>();

        }
    }
}
