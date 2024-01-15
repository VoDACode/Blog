using Blog.Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Server.Data
{
    public class BlogDbContext : DbContext
    {
        public DbSet<RecordModel> Records { get; set; } = null!;
        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<CommentModel> Comments { get; set; } = null!;
        public DbSet<FileModel> Files { get; set; } = null!;
        public DbSet<TagModel> Tags { get; set; } = null!;

        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecordModel>()
                .HasMany(r => r.Tags)
                .WithMany(t => t.Records);

            modelBuilder.Entity<RecordModel>()
                .HasMany(r => r.Files)
                .WithOne(f => f.Record)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.Records)
                .WithOne(r => r.Author)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentModel>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentModel>()
                .HasOne(c => c.Record)
                .WithMany(r => r.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
