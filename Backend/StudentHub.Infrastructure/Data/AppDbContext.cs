using Microsoft.EntityFrameworkCore;
using StudentHub.Application.Entities;

namespace StudentHub.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Business");
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectRating> ProjectRatings { get; set; }
        public DbSet<ProjectComment> ProjectComments { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
