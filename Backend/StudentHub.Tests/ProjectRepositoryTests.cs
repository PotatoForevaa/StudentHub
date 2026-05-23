using Microsoft.EntityFrameworkCore;
using StudentHub.Application.Entities;
using StudentHub.Infrastructure.Data;
using StudentHub.Infrastructure.Repositories;
using Xunit;

namespace StudentHub.Tests
{
    public class ProjectRepositoryTests
    {
        private AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }

        private async Task SeedEntitiesAsync(AppDbContext context)
        {
            var author = new User { Id = Guid.NewGuid(), Username = "author", FullName = "Project Author" };
            var commenter = new User { Id = Guid.NewGuid(), Username = "commenter", FullName = "Commenter User" };
            var reporter = new User { Id = Guid.NewGuid(), Username = "reporter", FullName = "Reporter User" };

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Test Project",
                Description = "Project for comments",
                AuthorId = author.Id,
                Author = author
            };

            var approvedComment = new Comment
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                Project = project,
                AuthorId = commenter.Id,
                Author = commenter,
                Content = "Approved comment",
                ModerationStatus = CommentModerationStatus.Approved,
                ModeratedBy = CommentModerationOrigin.Human,
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            };

            var pendingComment = new Comment
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                Project = project,
                AuthorId = commenter.Id,
                Author = commenter,
                Content = "Pending comment",
                ModerationStatus = CommentModerationStatus.Pending,
                ModeratedBy = CommentModerationOrigin.None,
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            };

            var toxicComment = new Comment
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                Project = project,
                AuthorId = commenter.Id,
                Author = commenter,
                Content = "Toxic comment",
                ModerationStatus = CommentModerationStatus.Toxic,
                ModeratedBy = CommentModerationOrigin.AI,
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            };

            var reportedComment = new Comment
            {
                Id = Guid.NewGuid(),
                ProjectId = project.Id,
                Project = project,
                AuthorId = commenter.Id,
                Author = commenter,
                Content = "Reported comment",
                ModerationStatus = CommentModerationStatus.Approved,
                ModeratedBy = CommentModerationOrigin.AI,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2),
                Reports = new List<CommentReport>
                {
                    new CommentReport { CommentId = Guid.NewGuid(), ReporterId = reporter.Id, CreatedAt = DateTime.UtcNow.AddMinutes(-3), Reporter = reporter }
                }
            };

            reportedComment.Reports[0].CommentId = reportedComment.Id;
            reportedComment.Reports[0].Comment = reportedComment;

            project.Comments.Add(approvedComment);
            project.Comments.Add(pendingComment);
            project.Comments.Add(toxicComment);
            project.Comments.Add(reportedComment);

            context.Users.AddRange(author, commenter, reporter);
            context.Projects.Add(project);
            context.Comments.AddRange(approvedComment, pendingComment, toxicComment, reportedComment);
            context.CommentReports.AddRange(reportedComment.Reports);

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddCommentAsync_Should_SavePendingCommentAndIncludeAuthor()
        {
            using var context = CreateContext("AddCommentTest");
            await SeedEntitiesAsync(context);
            var repository = new ProjectRepository(context);
            var existingProject = await context.Projects.FirstAsync();
            var commenter = await context.Users.FirstAsync(u => u.Username == "commenter");

            var newComment = new Comment
            {
                ProjectId = existingProject.Id,
                AuthorId = commenter.Id,
                Content = "New comment added for moderation",
                CreatedAt = DateTime.UtcNow
            };

            var result = await repository.AddCommentAsync(newComment);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(CommentModerationStatus.Pending, result.Value!.ModerationStatus);
            Assert.Equal(CommentModerationOrigin.None, result.Value.ModeratedBy);
            Assert.Equal(commenter.Username, result.Value.Author.Username);
            Assert.Equal("New comment added for moderation", result.Value.Content);
        }

        [Fact]
        public async Task GetCommentsByProjectIdAsync_Should_Return_OnlyApprovedByDefault()
        {
            using var context = CreateContext("ApprovedCommentsTest");
            await SeedEntitiesAsync(context);
            var repository = new ProjectRepository(context);
            var project = await context.Projects.FirstAsync();

            var result = await repository.GetCommentsByProjectIdAsync(project.Id);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
            Assert.All(result.Value, c => Assert.Equal(CommentModerationStatus.Approved, c.ModerationStatus));
        }

        [Fact]
        public async Task GetCommentsByProjectIdAsync_WithOnlyApprovedFalse_Should_Return_AllComments()
        {
            using var context = CreateContext("AllCommentsTest");
            await SeedEntitiesAsync(context);
            var repository = new ProjectRepository(context);
            var project = await context.Projects.FirstAsync();

            var result = await repository.GetCommentsByProjectIdAsync(project.Id, page: 1, pageSize: 10, onlyApproved: false);

            Assert.True(result.IsSuccess);
            Assert.Equal(4, result.Value.Count);
        }

        [Fact]
        public async Task GetReportedCommentsAsync_Should_Return_ReportedCommentsOrderedByReportsCount()
        {
            using var context = CreateContext("ReportedCommentsTest");
            await SeedEntitiesAsync(context);
            var repository = new ProjectRepository(context);

            var result = await repository.GetReportedCommentsAsync();

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
            Assert.Equal("Reported comment", result.Value[0].Content);
            Assert.Equal(1, result.Value[0].Reports.Count);
        }

        [Fact]
        public async Task AddCommentReportAsync_Should_AddReportAnd_Reject_Duplicate()
        {
            using var context = CreateContext("CommentReportTest");
            await SeedEntitiesAsync(context);
            var repository = new ProjectRepository(context);
            var comment = await context.Comments.FirstAsync(c => c.Content == "Approved comment");
            var reporter = await context.Users.FirstAsync(u => u.Username == "reporter");

            var firstResult = await repository.AddCommentReportAsync(new CommentReport
            {
                CommentId = comment.Id,
                ReporterId = reporter.Id,
                CreatedAt = DateTime.UtcNow
            });

            Assert.True(firstResult.IsSuccess);
            Assert.Equal(comment.Id, firstResult.Value.CommentId);

            var duplicateResult = await repository.AddCommentReportAsync(new CommentReport
            {
                CommentId = comment.Id,
                ReporterId = reporter.Id,
                CreatedAt = DateTime.UtcNow
            });

            Assert.False(duplicateResult.IsSuccess);
            Assert.Contains(duplicateResult.Errors, e => e.Field == "reporterId");
        }
    }
}
