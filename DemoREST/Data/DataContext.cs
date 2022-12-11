using DemoREST.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoREST.Data
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<PostTag>().HasKey(table => new { table.PostId, table.TagName });
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<RefreshToken> RefreshTokens{ get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<PostTag> PostTag { get; set; }
    }
}