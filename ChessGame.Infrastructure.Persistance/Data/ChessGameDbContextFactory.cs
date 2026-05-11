using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChessGame.Infrastructure.Persistance.Data
{
    public class ChessGameDbContextFactory(IConfiguration configuration)
        : IDesignTimeDbContextFactory<ChessGameDbContext>
    {
        public ChessGameDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChessGameDbContext>();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            return new ChessGameDbContext(optionsBuilder.Options);
        }
    }
}