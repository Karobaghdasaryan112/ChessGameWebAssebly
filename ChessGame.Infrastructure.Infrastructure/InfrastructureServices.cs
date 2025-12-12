using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.Services.BoardService;
using ChessGame.Core.Services.Services.HubServices;
using ChessGame.Core.Services.Services.Validations;
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

            services.AddScoped<GameHub>();

            services.AddSignalR()
        .AddHubOptions<ChessGame.Infrastructure.Infrastructure.Hubs.GameHub>(options =>
        {
            options.KeepAliveInterval = TimeSpan.FromSeconds(1);

            options.HandshakeTimeout = TimeSpan.FromSeconds(150000);
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
