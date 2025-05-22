using AnalysisService.Models;
using Microsoft.EntityFrameworkCore;

namespace AnalysisService.Data
{
    public class AnalysisDbContext : DbContext
    {
        public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : base(options)
        {
        }

        public DbSet<AnalysisEntity> Analyses { get; set; } = null!;
        public DbSet<ComparisonEntity> Comparisons { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure AnalysisEntity
            modelBuilder.Entity<AnalysisEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileId).IsRequired();
                entity.Property(e => e.ParagraphCount).IsRequired();
                entity.Property(e => e.WordCount).IsRequired();
                entity.Property(e => e.CharacterCount).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            });

            // Configure ComparisonEntity
            modelBuilder.Entity<ComparisonEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileId1).IsRequired();
                entity.Property(e => e.FileId2).IsRequired();
                entity.Property(e => e.SimilarityScore).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            });
        }
    }
}
