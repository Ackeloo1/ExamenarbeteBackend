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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ItemModel>().ToTable("tblItem");
            builder.Entity<PlayerModel>().ToTable("tblPlayer");
        }
    }
}
