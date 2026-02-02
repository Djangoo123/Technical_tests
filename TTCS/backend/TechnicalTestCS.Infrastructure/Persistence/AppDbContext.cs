using Microsoft.EntityFrameworkCore;

namespace TechnicalTestCS.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ArticleStatusEntity> ArticleStatuses => Set<ArticleStatusEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleStatusEntity>(b =>
            {
                b.ToTable("article_statuses");
                b.HasKey(x => x.ArticleId);
                b.Property(x => x.Status).HasConversion<string>(); // "Draft", "Pending"...
                b.Property(x => x.UpdatedAtUtc).IsRequired();
            });
        }
    }
}
