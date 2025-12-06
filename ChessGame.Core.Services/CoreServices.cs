using ChessGame.Core.Services.MediatR.Handlers.Commands;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            //services.AddScoped<BaseHubService<GameHub>>();
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

            services.AddLogging();
        }
    }
}
