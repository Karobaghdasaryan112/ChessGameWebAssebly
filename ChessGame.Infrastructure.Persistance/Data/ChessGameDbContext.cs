using ChessGame.Domain.Domain.Contracts;
using ChessGame.Domain.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChessGame.Infrastructure.Persistance.Data
{
    /// <summary>
    /// Represents the Entity Framework Core database context for managing chess games and their histories.
    /// </summary>
    /// <remarks>This context provides access to chess game data and related history records using Entity
    /// Framework Core. It configures entity sets for games and their histories, and automatically updates timestamp
    /// fields on tracked entities that implement the IEntity interface when changes are saved. This class is intended
    /// to be used with dependency injection and should be configured with the appropriate database provider and
    /// options.</remarks>
    public class ChessGameDbContext : DbContext
    {
        public ChessGameDbContext(DbContextOptions<ChessGameDbContext> options)
            : base(options)
        {
        }

        public DbSet<Game> ChessGames { get; set; }
        public DbSet<ChessGameHistory> ChessGamesHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<IEntity<object>>();

            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreateDate = now;
                    entry.Entity.UpdateDate = now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdateDate = now;
                }
            }
        }
    }

}
