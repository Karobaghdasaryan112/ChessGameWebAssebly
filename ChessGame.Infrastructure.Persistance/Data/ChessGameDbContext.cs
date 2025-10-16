using ChessGame.Domain.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChessGame.Infrastructure.Persistance.Data
{
    public class ChessGameDbContext : DbContext
    {
        public ChessGameDbContext(DbContextOptions<ChessGameDbContext> options) : base(options)
        {
        }
        public DbSet<Game> ChessGames { get; set; }
        public DbSet<ChessGameHistory> ChessGamesHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Game>()
                .HasMany(g => g.History)
                .WithOne(h => h.Game)
                .HasForeignKey(h => h.GameId);
        }
        

    }
}
