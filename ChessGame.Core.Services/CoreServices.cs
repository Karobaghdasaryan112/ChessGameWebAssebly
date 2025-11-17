using ChessGame.Core.Services.Contracts.BoardServices;
using ChessGame.Core.Services.Contracts.Hub;
using ChessGame.Core.Services.MediatR.Handlers.Queries;
using ChessGame.Core.Services.Services.Board;
using ChessGame.Core.Services.Services.BoardService;
using ChessGame.Core.Services.Services.HubServices;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SharedResources.Validation.ChessGameValidations;

namespace ChessGame.Core.Services
{
    public static class CoreServices
    {
        public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(GetMoveCommnadHandler)));

            services.AddValidatorsFromAssemblyContaining<BoardInitializeDTOValidator>();
            services.AddSignalR()
            .AddNewtonsoftJsonProtocol(options =>
            {
                options.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            }); 
            services.AddScoped(typeof(BaseHubService<>));
            services.AddSingleton(typeof(IConnectionService<>), typeof(ConnetionService<>));
            services.AddSingleton(typeof(IInvitationService<>), typeof(InvitationService<>));
            services.AddSingleton(typeof(IGameService<>), typeof(GameService<>));

            services.AddScoped<IBoardService, BoardService>();
            services.AddScoped<IBlockService, BlockService>();

            services.AddLogging();
        }
    }
}
