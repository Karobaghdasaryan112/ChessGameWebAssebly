using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace IdentityServer.Data;

public class ContextIdentityServer : IdentityDbContext<IdentityUser>
{
    public ContextIdentityServer(DbContextOptions<ContextIdentityServer> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
       
    }
    public DbSet<OpenIddictEntityFrameworkCoreApplication> Applications {  get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreScope> Scopes {  get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreToken> Tokens {  get; set; }



}
