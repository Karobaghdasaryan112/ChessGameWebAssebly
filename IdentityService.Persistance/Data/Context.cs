using IdentityService.Domain.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class IdentityContext : IdentityDbContext<ApplicationUser>
{
    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>(b =>
        {
            b.Property(r => r.ConcurrencyStamp).HasMaxLength(256);
        });

        builder.Entity<ApplicationUser>(b =>
        {
            b.Property(u => u.UserName).HasMaxLength(256);
            b.Property(u => u.NormalizedUserName).HasMaxLength(256);
            b.Property(u => u.Email).HasMaxLength(256);
            b.Property(u => u.NormalizedEmail).HasMaxLength(256);
            b.Property(u => u.ConcurrencyStamp).HasMaxLength(256);
        });

        builder.Entity<IdentityUserLogin<string>>(b =>
        {
            b.Property(l => l.ProviderKey).HasMaxLength(256);
            b.Property(l => l.LoginProvider).HasMaxLength(256);
        });

        builder.Entity<IdentityUserToken<string>>(b =>
        {
            b.Property(t => t.LoginProvider).HasMaxLength(256);
            b.Property(t => t.Name).HasMaxLength(256);
        });

        builder.Entity<IdentityUserRole<string>>(b =>
        {
            b.HasKey(r => new { r.UserId, r.RoleId });
        });

        builder.Entity<IdentityUserClaim<string>>(b =>
        {
            b.Property(c => c.ClaimType).HasMaxLength(256);
            b.Property(c => c.ClaimValue).HasMaxLength(256);
        });

        builder.Entity<IdentityRoleClaim<string>>(b =>
        {
            b.Property(rc => rc.ClaimType).HasMaxLength(256);
            b.Property(rc => rc.ClaimValue).HasMaxLength(256);
        });
    }

}
