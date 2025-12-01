using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.MediatR.Handlers.Commands;
using ChessGame.Core.Services.Services.BoardService;
using ChessGame.Core.Services.Services.HubServices;
using ChessGame.Core.Services.Services.Validations;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.Validation.ChessGameValidations;
using SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;
using SharedResources.Validation.ChessGameValidations.ResponseValidations.GameResponses;

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

            services.AddValidatorsFromAssembly(typeof(SubmitMoveRequestDTOValidator).Assembly, ServiceLifetime.Scoped);
            services.AddValidatorsFromAssembly(typeof(SubmitMoveResponseDTOValidator).Assembly, ServiceLifetime.Scoped);


            services.AddScoped(typeof(BaseHubService<>));
            services.AddScoped(typeof(IConnectionService<>), typeof(ConnetionService<>));
            services.AddScoped(typeof(IInvitationService<>), typeof(InvitationService<>));
            services.AddScoped(typeof(IGameService<>), typeof(GameService<>));
            services.AddScoped<GameValidationService>();
            services.AddScoped<IBoardService, BoardService>();

            services.AddLogging();
        }
    }
}
