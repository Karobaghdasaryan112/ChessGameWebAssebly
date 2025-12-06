using ChessGame.Core.Services.Contracts.Repositories;
using ChessGame.Core.Services.MediatR.Handlers.Commands;
using ChessGame.Infrastructure.Persistance.Data;
using ChessGame.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChessGame.Infrastructure.Infrastructure.Persistance
{
    public static class PersistanceServices
    {
        public static void AddPersistanceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ChessGameDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(BoardInitializeCommandHandler).Assembly
                );

                cfg.Lifetime = ServiceLifetime.Scoped;
            });
            services.AddScoped<IChessGameRepository, ChessGameRepository>();
            services.AddScoped<IChessGameHistoryRepository, ChessGameHistoryRepository>();
        }
    }
}
