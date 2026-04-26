using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Services.BoardService;
using ChessGame.Core.Services.Services.HubServices;
using ChessGame.Infrastructure.Infrastructure.Hubs;
using ChessGame.Infrastructure.Infrastructure.HubServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ChessGame.Infrastructure.Infrastructure
{
    public static class InfrastructureServices
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSignalR()
        .AddHubOptions<GameHub>(options =>
        {
            options.KeepAliveInterval = TimeSpan.FromSeconds(150000);
            options.HandshakeTimeout = TimeSpan.FromSeconds(1500000);
            options.EnableDetailedErrors = true;
        });
            services.AddScoped(typeof(BaseHubService));
            services.AddScoped(typeof(IConnectionService), typeof(ConnectionService));
            services.AddScoped(typeof(IInvitationService), typeof(InvitationService));
            services.AddScoped(typeof(IGameService), typeof(GameService));
            services.AddScoped<GenericValidationService>();
            services.AddScoped<IBoardService, BoardService>();
            services.AddScoped<IHistoryWidgetService, HistoryWidgetService>();
        }
    }
}
