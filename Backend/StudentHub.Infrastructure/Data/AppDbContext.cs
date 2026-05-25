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

            // ProjectCategory - composite PK
            builder.Entity<ProjectCategory>()
                .HasKey(pc => new { pc.ProjectId, pc.CategoryId });

            builder.Entity<ProjectCategory>()
                .HasOne(pc => pc.Project)
                .WithMany(p => p.ProjectCategories)
                .HasForeignKey(pc => pc.ProjectId);

            builder.Entity<ProjectCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProjectCategories)
                .HasForeignKey(pc => pc.CategoryId);

            // ProjectTag - composite PK
            builder.Entity<ProjectTag>()
                .HasKey(pt => new { pt.ProjectId, pt.TagId });

            builder.Entity<ProjectTag>()
                .HasOne(pt => pt.Project)
                .WithMany(p => p.ProjectTags)
                .HasForeignKey(pt => pt.ProjectId);

            builder.Entity<ProjectTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.ProjectTags)
                .HasForeignKey(pt => pt.TagId);

            // Criterion
            builder.Entity<Criterion>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Criteria)
                .HasForeignKey(c => c.CategoryId);

            // CriterionScore
            builder.Entity<CriterionScore>()
                .HasOne(cs => cs.Project)
                .WithMany(p => p.CriterionScores)
                .HasForeignKey(cs => cs.ProjectId);

            builder.Entity<CriterionScore>()
                .HasOne(cs => cs.Criterion)
                .WithMany(c => c.Scores)
                .HasForeignKey(cs => cs.CriterionId);
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReport> CommentReports { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserMute> UserMutes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Criterion> Criteria { get; set; }
        public DbSet<CriterionScore> CriterionScores { get; set; }
        public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public DbSet<ProjectTag> ProjectTags { get; set; }

        public override int SaveChanges()
        {
            ConvertDateTimeToUtc();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ConvertDateTimeToUtc();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ConvertDateTimeToUtc()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                foreach (var property in entry.Properties)
                {
                    if (property.Metadata.ClrType == typeof(DateTime) || property.Metadata.ClrType == typeof(DateTime?))
                    {
                        var dateTime = (DateTime?)property.CurrentValue;
                        if (dateTime.HasValue && dateTime.Value.Kind != DateTimeKind.Utc)
                        {
                            property.CurrentValue = DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
                        }
                    }
                }
            }
        }
    }
}
