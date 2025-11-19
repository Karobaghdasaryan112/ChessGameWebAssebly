//using Microsoft.AspNetCore.Hosting;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;
//using System.Reflection;

//namespace ChessGame.Infrastructure.Persistance.Data
//{
//    public class ChessGameDbContextFactory : IDesignTimeDbContextFactory<ChessGameDbContext>
//    {
//        public ChessGameDbContext CreateDbContext(string[] args)
//        {
//            var optionsBuilder = new DbContextOptionsBuilder<ChessGameDbContext>();

//            optionsBuilder.UseSqlite(@"Data Source=C:\Users\karapet.baghdasaryan\source\repos\\ChessGameWebAssembly\ChessGameWebAssebly\IdentityService.Persistance\Identity.db");
//            return new ChessGameDbContext(optionsBuilder.Options);
//        }
//    }
//}
