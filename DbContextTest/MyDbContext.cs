using Microsoft.EntityFrameworkCore;
using TestMediatR1.Item.Models;
using TestMediatR1.Player.Models;

namespace TestMediatR1.DbContext
{
    public class MyDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base (options)
        {
            
        }

        public DbSet<ItemModel> tblItem { get; set; }
        public DbSet<PlayerModel> tblPlayer { get; set; }
        public DbSet<PlayerScore> tblScore { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ItemModel>(item =>
            {
                item.HasIndex(i => i.Name).IsUnique();
            });

            builder.Entity<PlayerModel>(player =>
            {
                player.HasIndex(p => p.Username).IsUnique();
            });

            builder.Entity<PlayerScore>(score =>
            {
                score.HasMany(p => p.Players).WithOne(s => s.Score).HasForeignKey(p => p.ScoreId).OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
