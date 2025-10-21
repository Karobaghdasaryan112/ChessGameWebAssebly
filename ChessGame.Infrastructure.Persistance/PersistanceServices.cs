using ChessGame.Core.Services.Contracts.Repositories;
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
            services.AddDbContext<ChessGameDbContext>(option =>
                option.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IChessGameHistoryRepository, ChessGameHistoryRepository>();
            services.AddScoped<IChessGameRepository, ChessGameRepository>();
        }
    }
}
