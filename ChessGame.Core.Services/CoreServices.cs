using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.MediatR.Handlers.Commands;
using ChessGame.Core.Services.Services.BoardService;
using ChessGame.Core.Services.Services.HubServices;
using ChessGame.Core.Services.Services.Validations;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests;
using SharedResources.Validation.ChessGameValidations.RequestValidations.GameRequests;
using SharedResources.Validation.ChessGameValidations.RequestValidations.HistoryWidgetsRequests;
using SharedResources.Validation.ChessGameValidations.ResponseValidations.GameResponses;
using IHistoryWidgetService = ChessGame.Core.Services.Contracts.BoardServices.IHistoryWidgetService;

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
            services.AddValidatorsFromAssembly(typeof(GetAllHistoryWidgetsRequestDTOValidator).Assembly, ServiceLifetime.Scoped);
            services.AddValidatorsFromAssemblyContaining<GetAllHistoryWidgetsRequestDTOValidator>();
            services.AddValidatorsFromAssemblyContaining<IsKingCheckedRequestDTOValidation>();
            services.AddValidatorsFromAssemblyContaining<IsKingMateRequestDTOValidation>();
            services.AddValidatorsFromAssembly(typeof(SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests.AddUserConnectionRequestDTOValidation).Assembly, ServiceLifetime.Scoped);
            services.AddValidatorsFromAssembly(typeof(SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests.GetUserConnectionRequestDTOValidation).Assembly, ServiceLifetime.Scoped);
            services.AddValidatorsFromAssembly(typeof(SharedResources.Validation.ChessGameValidations.RequestValidations.ConnectionRequests.RemoveUserConnectionRequestDTOValidation).Assembly, ServiceLifetime.Scoped);
            services.AddValidatorsFromAssembly(typeof(IsKingCheckedRequestDTOValidation).Assembly);
            services.AddValidatorsFromAssembly(typeof(GetAllHistoryWidgetsRequestDTOValidator).Assembly);
            services.AddScoped(typeof(BaseHubService<>));
            services.AddScoped(typeof(IConnectionService<>), typeof(ConnetionService<>));
            services.AddScoped(typeof(IInvitationService<>), typeof(InvitationService<>));
            services.AddScoped(typeof(IGameService<>), typeof(GameService<>));
            services.AddScoped<GenericValidationService>();
            services.AddScoped<IBoardService, BoardService>();
            services.AddScoped<IHistoryWidgetService, HistoryWidgetService>();
            services.AddLogging();
        }
    }
}
