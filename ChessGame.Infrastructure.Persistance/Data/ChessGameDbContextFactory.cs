using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ChessGame.Infrastructure.Persistance.Data
{
    public class ChessGameDbContextFactory : IDesignTimeDbContextFactory<ChessGameDbContext>
    {
        public ChessGameDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChessGameDbContext>();

            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS01;Database=ChessGameDb;Trusted_Connection=True;TrustServerCertificate=True;");
            return new ChessGameDbContext(optionsBuilder.Options);
        }
    }
}
