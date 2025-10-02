using StudentHub.Domain.Entities;
using StudentHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StudentHub.Infrastructure
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<PostRating> PostRatings { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasOne<AppUser>()
                      .WithMany()
                      .HasForeignKey(p => p.AuthorId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<PostRating>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasOne<AppUser>()
                      .WithMany()
                      .HasForeignKey(p => p.AuthorId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasOne<AppUser>()
                      .WithMany()
                      .HasForeignKey(p => p.AuthorId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
