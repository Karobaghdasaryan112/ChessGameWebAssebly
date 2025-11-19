using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.MediatR.Handlers.Commands;
using ChessGame.Core.Services.Services.Board;
using ChessGame.Core.Services.Services.BoardService;
using ChessGame.Core.Services.Services.HubServices;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.Validation.ChessGameValidations;

namespace ChessGame.Core.Services
{
    public static class CoreServices
    {
        public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(BoardInitializeCommandHandler).Assembly
                );

                cfg.Lifetime = ServiceLifetime.Scoped;
            });

            services.AddValidatorsFromAssemblyContaining<BoardInitializeDTOValidator>(ServiceLifetime.Scoped);
            services.AddSignalR()
                .AddJsonProtocol(options =>
                {
                });
            services.AddScoped(typeof(BaseHubService<>));
            services.AddScoped(typeof(IConnectionService<>), typeof(ConnetionService<>));
            services.AddScoped(typeof(IInvitationService<>), typeof(InvitationService<>));
            services.AddScoped(typeof(IGameService<>), typeof(GameService<>));

            services.AddScoped<IBoardService, BoardService>();
            services.AddScoped<IBlockService, BlockService>();

            services.AddLogging();
        }
    }
}
