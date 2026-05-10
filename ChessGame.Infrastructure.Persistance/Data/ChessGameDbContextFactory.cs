using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ChessGame.Infrastructure.Persistance.Data
{
    public class ChessGameDbContextFactory : IDesignTimeDbContextFactory<ChessGameDbContext>
    {
        public ChessGameDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChessGameDbContext>();

            optionsBuilder.UseSqlServer("Server=sqlserver,1433;Database=master;User Id=sa;Password=Karokar3.;TrustServerCertificate=True;");
            return new ChessGameDbContext(optionsBuilder.Options);
        }
    }
}
