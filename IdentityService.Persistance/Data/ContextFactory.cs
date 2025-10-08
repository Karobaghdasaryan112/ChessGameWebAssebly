using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace IdentityService.Persistance.Data
{
    public class ContextFactory : IDesignTimeDbContextFactory<IdentityContext>
    {
        public IdentityContext CreateDbContext(string[] args)
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string? jsonFile = @"C:\Users\karapet.baghdasaryan\source\repos\ChessGameWebAssembly\ChessGameWebAssebly\IdentityService.API.IdentityAPI\appsettings.json";


            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile(jsonFile, optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<IdentityContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");


            optionsBuilder.UseSqlite(connectionString);

            return new IdentityContext(optionsBuilder.Options);
        }
    }
}
