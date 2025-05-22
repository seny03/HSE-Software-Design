using FileService.Models;
using Microsoft.EntityFrameworkCore;

namespace FileService.Data
{
    public class FileDbContext : DbContext
    {
        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
        {
        }

        public DbSet<FileEntity> Files { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure FileEntity
            modelBuilder.Entity<FileEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Filename).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(1024);
                entity.Property(e => e.Size).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            });
        }
    }
}
