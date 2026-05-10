using BlazorServerSideClient.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerSideClient.Helpers;

public class MigrationApplierService
{
    public void ApplyMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        try
        {
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}