using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using Microsoft.Extensions.Logging;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Entities;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Application.Interfaces.Services;
using StudentHub.Application.Interfaces.UseCases;

namespace StudentHub.Application.UseCases
{
    public class ProjectUseCase : IProjectUseCase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IFileStorageService _fileService;
        private readonly IToxicFilterService _toxicFilterService;
        private readonly CommentSettings _commentSettings;
        private readonly ILogger<ProjectUseCase> _logger;

        public ProjectUseCase(IProjectRepository projectRepository, IFileStorageService fileService, IToxicFilterService toxicFilterService, CommentSettings commentSettings, ILogger<ProjectUseCase> logger)
        {
            _projectRepository = projectRepository;
            _fileService = fileService;
            _toxicFilterService = toxicFilterService;
            _commentSettings = commentSettings;
            _logger = logger;
        }

        private async Task<ProjectCommentDto> ToCommentDto(Comment comment)
        {
            var userScore = await _projectRepository.GetUserScoreForProjectAsync(comment.AuthorId, comment.ProjectId);

            return new ProjectCommentDto(
                Id: comment.Id,
                AuthorId: comment.AuthorId,
                AuthorUsername: comment.Author.Username,
                AuthorFullName: comment.Author.FullName,
                AuthorProfilePicturePath: string.IsNullOrEmpty(comment.Author.ProfilePicturePath) ? null : comment.Author.ProfilePicturePath,
                Content: comment.Content,
                CreatedAt: comment.CreatedAt,
                UserScore: userScore,
                ProjectId: comment.ProjectId,
                ProjectName: comment.Project?.Name,
                ModerationStatus: comment.ModerationStatus.ToString(),
                ModeratedBy: comment.ModeratedBy.ToString(),
                ReportCount: comment.Reports?.Count ?? 0,
                AppealStatus: comment.AppealStatus.ToString(),
                AppealMessage: comment.AppealMessage
            );
        }

        public async Task<Result<ProjectDto?>> CreateAsync(CreateProjectCommand command)
        {
            var filePaths = new List<string>();
            if (command.Files?.Count > 0)
                foreach (var file in command.Files)
                {
                    var fileResult = await _fileService.SaveFileAsync(file, "");
                    filePaths.Add(fileResult.Value);
                }

            var project = new Project
            {
                AuthorId = command.AuthorId,
                Name = command.Name,
                Description = command.Description,
                ExternalUrl = string.IsNullOrEmpty(command.Url) ? null : new Uri(command.Url),
                Attachments = filePaths.Select(fp => new Attachment { Path = fp }).ToList()
            };

            var result = await _projectRepository.AddAsync(project);
            if (!result.IsSuccess) return Result<ProjectDto?>.Failure(result.Errors);
            var value = result.Value;

            // Add category and tags if provided
            if (command.CategoryId.HasValue)
                await _projectRepository.AddProjectCategoriesAsync(project.Id, new List<Guid> { command.CategoryId.Value });
            if (command.TagIds?.Count > 0)
                await _projectRepository.AddProjectTagsAsync(project.Id, command.TagIds);

            var projectDto = new ProjectDto(value.Id, value.Name, value.Description, filePaths, AuthorName: value.Author.FullName, AuthorUsername: value.Author.Username, AuthorProfilePicturePath: $"api/users/by-username/{value.Author.Username}/profile-picture", CreationDate: value.CreatedAt);
            return Result<ProjectDto?>.Success(projectDto);
        }

        public async Task<Result<double>> AddScoreAsync(AddProjectScoreCommand command)
        {
            if (command.Score < 1 || command.Score > 5)
                return Result<double>.Failure("Score must be between 1 and 5", "score", ErrorType.Validation);

            var rating = new Rating
            {
                AuthorId = command.AuthorId,
                ProjectId = command.ProjectId,
                Score = command.Score,
                DateTime = DateTime.UtcNow
            };

            var result = await _projectRepository.AddRatingAsync(rating);
            return result;
        }

        public async Task<Result<double>> GetAverageRatingAsync(Guid projectId)
        {
            return await _projectRepository.GetAverageRatingAsync(projectId);
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var result = await _projectRepository.DeleteAsync(id);
            return result;
        }

        public async Task<Result<List<ProjectDto>>> GetAllAsync(int page = 0, int pageSize = 0)
        {
            var projectListResult = await _projectRepository.GetAllAsync(page, pageSize);
            if (!projectListResult.IsSuccess) return Result<List<ProjectDto>>.Failure(projectListResult.Errors, projectListResult.ErrorType);
            var projectList = projectListResult.Value;
            var dtoList = new List<ProjectDto>();

            foreach (var p in projectList)
            {
                var avgRatingResult = await GetAverageRatingAsync(p.Id);
                double? avgRating = avgRatingResult.IsSuccess ? avgRatingResult.Value : null;

                var categories = p.ProjectCategories?.Select(pc => new CategoryDto(pc.CategoryId, pc.Category?.Name ?? "")).ToList();
                var tags = p.ProjectTags?.Select(pt => new TagDto(pt.TagId, pt.Tag?.Name ?? "")).ToList();

                var dto = new ProjectDto(
                    Name: p.Name,
                    Description: p.Description,
                    Files: p.Attachments.Select(i => i.Path).ToList(),
                    Id: p.Id,
                    AuthorName: p.Author.FullName,
                    AuthorUsername: p.Author.Username,
                    AuthorProfilePicturePath: $"api/users/by-username/{p.Author.Username}/profile-picture",
                    CreationDate: p.CreatedAt,
                    AverageRating: avgRating,
                    Categories: categories,
                    Tags: tags
                );
                dtoList.Add(dto);
            }

            return Result<List<ProjectDto>>.Success(dtoList);
        }

        public async Task<Result<PaginatedResponse<AdminProjectDto>>> SearchProjectsAsync(string? search, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (projects, totalCount) = await _projectRepository.SearchProjectsAsync(search, page, pageSize);
            var dtos = projects.Select(p => new AdminProjectDto(
                p.Id,
                p.Name,
                p.Description,
                p.Author.Username,
                p.Author.FullName,
                p.CreatedAt)).ToList();

            return Result<PaginatedResponse<AdminProjectDto>>.Success(new PaginatedResponse<AdminProjectDto>(
                dtos,
                page,
                pageSize,
                totalCount,
                (int)Math.Ceiling(totalCount / (double)pageSize)));
        }

        public async Task<Result<List<ProjectDto>>> GetProjectsByAuthorIdAsync(Guid authorId)
        {
            var projectListResult = await _projectRepository.GetProjectsByAuthorIdAsync(authorId);
            if (!projectListResult.IsSuccess) return Result<List<ProjectDto>>.Failure(projectListResult.Errors, projectListResult.ErrorType);
            var projectList = projectListResult.Value;
            var dtoList = new List<ProjectDto>();

            foreach (var p in projectList)
            {
                var avgRatingResult = await GetAverageRatingAsync(p.Id);
                double? avgRating = avgRatingResult.IsSuccess ? avgRatingResult.Value : null;

                var categories = p.ProjectCategories?.Select(pc => new CategoryDto(pc.CategoryId, pc.Category?.Name ?? "")).ToList();
                var tags = p.ProjectTags?.Select(pt => new TagDto(pt.TagId, pt.Tag?.Name ?? "")).ToList();

                var dto = new ProjectDto(
                    Name: p.Name,
                    Description: p.Description,
                    Files: p.Attachments.Select(i => i.Path).ToList(),
                    Id: p.Id,
                    AuthorName: p.Author.FullName,
                    AuthorUsername: p.Author.Username,
                    AuthorProfilePicturePath: $"api/users/by-username/{p.Author.Username}/profile-picture",
                    CreationDate: p.CreatedAt,
                    AverageRating: avgRating,
                    Categories: categories,
                    Tags: tags
                );
                dtoList.Add(dto);
            }

            return Result<List<ProjectDto>>.Success(dtoList);
        }

        public async Task<Result<ProjectDto?>> GetByIdAsync(Guid id)
        {
            var projectResult = await _projectRepository.GetByIdAsync(id);
            if (!projectResult.IsSuccess) return Result<ProjectDto?>.Failure(projectResult.Errors);
            var project = projectResult.Value;

            var avgRatingResult = await GetAverageRatingAsync(id);
            double? avgRating = avgRatingResult.IsSuccess ? avgRatingResult.Value : null;

            var commentsResult = await GetCommentsByProjectIdAsync(id);
            List<ProjectCommentDto>? comments = commentsResult.IsSuccess ? commentsResult.Value : null;

            var categories = project.ProjectCategories?.Select(pc => new CategoryDto(pc.CategoryId, pc.Category?.Name ?? "")).ToList();
            var tags = project.ProjectTags?.Select(pt => new TagDto(pt.TagId, pt.Tag?.Name ?? "")).ToList();
            var criterionScores = project.CriterionScores?.Select(cs => new CriterionScoreDto(
                cs.CriterionId,
                cs.Criterion?.Name ?? "",
                cs.Criterion?.Category?.Name ?? "",
                cs.Score,
                cs.Comment,
                cs.Teacher?.FullName ?? "",
                cs.CreatedAt
            )).ToList();

            var projectDto = new ProjectDto(
                Id: project.Id,
                Name: project.Name,
                Description: project.Description,
                Files: project.Attachments.Select(i => i.Path).ToList(),
                AuthorName: project.Author.FullName,
                AuthorUsername: project.Author.Username,
                AuthorProfilePicturePath: $"api/users/by-username/{project.Author.Username}/profile-picture",
                CreationDate: project.CreatedAt,
                AverageRating: avgRating,
                Comments: comments,
                Categories: categories,
                Tags: tags,
                CriterionScores: criterionScores
            );

            return Result<ProjectDto?>.Success(projectDto);
        }

        public async Task<Result<Stream>> GetImageAsync(string path)
        {
            return await _fileService.GetFileAsync(path);
        }


        public async Task<Result<List<string>>> GetImageListByIdAsync(Guid id)
        {
            return await _projectRepository.GetImageListByIdAsync(id);
        }

        public async Task<Result<ProjectDto?>> UpdateAsync(UpdateProjectCommand command)
        {
            var projectResult = await _projectRepository.GetByIdAsync(command.ProjectId);
            if (!projectResult.IsSuccess)
                return Result<ProjectDto?>.Failure(projectResult.Errors);

            var project = projectResult.Value!;

            project.Name = command.Name;
            project.Description = command.Description;
            project.ExternalUrl = string.IsNullOrWhiteSpace(command.ExternalUrl)
                ? null
                : new Uri(command.ExternalUrl);

            var filePaths = new List<string>();
            if (command.Files?.Count > 0)
                foreach (var file in command.Files)
                {
                    var fileResult = await _fileService.SaveFileAsync(file, "");
                    filePaths.Add(fileResult.Value);
                }

            project.Attachments.Clear();

            foreach (var path in filePaths)
            {
                project.Attachments.Add(new Attachment { Path = path, Project = project });
            }

            // Update category and tags if provided
            if (command.CategoryId.HasValue)
            {
                await _projectRepository.ClearProjectCategoriesAsync(command.ProjectId);
                await _projectRepository.AddProjectCategoriesAsync(command.ProjectId, new List<Guid> { command.CategoryId.Value });
            }

            if (command.TagIds != null)
            {
                await _projectRepository.ClearProjectTagsAsync(command.ProjectId);
                if (command.TagIds.Count > 0)
                    await _projectRepository.AddProjectTagsAsync(command.ProjectId, command.TagIds);
            }

            var updateResult = await _projectRepository.UpdateAsync(project);
            if (!updateResult.IsSuccess) return Result<ProjectDto?>.Failure(updateResult.Errors);

            return Result<ProjectDto?>.Success(new ProjectDto(
                Id: project.Id,
                Name: project.Name,
                Description: project.Description,
                Files: project.Attachments.Select(i => i.Path).ToList(),
                AuthorName: project.Author.FullName,
                AuthorUsername: project.Author.Username,
                AuthorProfilePicturePath: project.Author.ProfilePicturePath,
                CreationDate: project.CreatedAt
            ));
        }

        public async Task<Result<ProjectCommentDto>> AddCommentAsync(CreateProjectCommentCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Content))
                return Result<ProjectCommentDto>.Failure("Comment content cannot be empty", "content", ErrorType.Validation);

            if (command.Content.Length < _commentSettings.MinLength)
                return Result<ProjectCommentDto>.Failure($"Comment must be at least {_commentSettings.MinLength} characters.", "content", ErrorType.Validation);

            if (command.Content.Length > _commentSettings.MaxLength)
                return Result<ProjectCommentDto>.Failure($"Comment must be at most {_commentSettings.MaxLength} characters.", "content", ErrorType.Validation);

            var comment = new Comment
            {
                ProjectId = command.ProjectId,
                AuthorId = command.AuthorId,
                Content = command.Content,
                CreatedAt = DateTime.UtcNow,
                ModerationStatus = CommentModerationStatus.Pending,
                ModeratedBy = CommentModerationOrigin.None
            };

            var saveResult = await _projectRepository.AddCommentAsync(comment);
            if (!saveResult.IsSuccess) return Result<ProjectCommentDto>.Failure(saveResult.Errors);

            var savedComment = saveResult.Value!;
            if (savedComment.Author == null)
                return Result<ProjectCommentDto>.Failure("Failed to load comment author", "author", ErrorType.NotFound);

            var asyncResult = await _toxicFilterService.PredictAsync(command.Content, savedComment.Id);
            if (asyncResult.IsSuccess && !string.IsNullOrWhiteSpace(asyncResult.Value))
            {
                savedComment.ToxicFilterTaskId = asyncResult.Value;
                await _projectRepository.UpdateCommentAsync(savedComment);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to enqueue toxic filter task for comment {CommentId}: {Errors}",
                    savedComment.Id,
                    string.Join("; ", asyncResult.Errors.Select(e => e.Message)));
            }

            var commentDto = await ToCommentDto(savedComment);
            return Result<ProjectCommentDto>.Success(commentDto);
        }

        public async Task<Result> ReportCommentAsync(Guid commentId, Guid reporterId)
        {
            var report = new CommentReport
            {
                CommentId = commentId,
                ReporterId = reporterId,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _projectRepository.AddCommentReportAsync(report);
            if (!result.IsSuccess) return Result.Failure(result.Errors, result.ErrorType);

            return Result.Success();
        }

        public async Task<Result<List<ProjectCommentDto>>> GetCommentsByProjectIdAsync(Guid projectId)
        {
            var result = await _projectRepository.GetCommentsByProjectIdAsync(projectId);
            if (!result.IsSuccess) return Result<List<ProjectCommentDto>>.Failure(result.Errors);

            var comments = result.Value;
            var commentDtos = new List<ProjectCommentDto>();

            foreach (var comment in comments)
            {
                commentDtos.Add(await ToCommentDto(comment));
            }

            return Result<List<ProjectCommentDto>>.Success(commentDtos);
        }

        public async Task<Result<PaginatedResponse<ProjectCommentDto>>> GetCommentsByProjectIdAsync(Guid projectId, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var result = await _projectRepository.GetCommentsByProjectIdAsync(projectId, page, pageSize);
            if (!result.IsSuccess) return Result<PaginatedResponse<ProjectCommentDto>>.Failure(result.Errors, result.ErrorType);

            var commentDtos = new List<ProjectCommentDto>();
            foreach (var comment in result.Value)
            {
                commentDtos.Add(await ToCommentDto(comment));
            }

            var totalCount = await _projectRepository.CountCommentsByProjectIdAsync(projectId);
            return Result<PaginatedResponse<ProjectCommentDto>>.Success(new PaginatedResponse<ProjectCommentDto>(
                commentDtos,
                page,
                pageSize,
                totalCount,
                (int)Math.Ceiling(totalCount / (double)pageSize)));
        }

        public async Task<Result<List<ProjectCommentDto>>> GetCommentsByAuthorIdAsync(Guid authorId)
        {
            var result = await _projectRepository.GetCommentsByAuthorIdAsync(authorId);
            if (!result.IsSuccess) return Result<List<ProjectCommentDto>>.Failure(result.Errors);

            var comments = result.Value;
            var commentDtos = new List<ProjectCommentDto>();

            foreach (var comment in comments)
            {
                commentDtos.Add(await ToCommentDto(comment));
            }

            return Result<List<ProjectCommentDto>>.Success(commentDtos);
        }

        public async Task<Result<PaginatedResponse<ProjectCommentDto>>> GetModerationCommentsAsync(string queue, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            Result<List<Comment>> result;
            int totalCount;

            if (queue == "reported")
            {
                result = await _projectRepository.GetReportedCommentsAsync(page, pageSize);
                totalCount = await _projectRepository.CountReportedCommentsAsync();
            }
            else if (queue == "ai-toxic")
            {
                result = await _projectRepository.GetCommentsByModerationStatusAsync(
                    CommentModerationStatus.Toxic,
                    CommentModerationOrigin.AI,
                    page,
                    pageSize);
                totalCount = await _projectRepository.CountCommentsByModerationStatusAsync(
                    CommentModerationStatus.Toxic,
                    CommentModerationOrigin.AI);
            }
            else if (queue == "moderator-toxic")
            {
                result = await _projectRepository.GetCommentsByModerationStatusAsync(
                    CommentModerationStatus.Toxic,
                    CommentModerationOrigin.Human,
                    page,
                    pageSize);
                totalCount = await _projectRepository.CountCommentsByModerationStatusAsync(
                    CommentModerationStatus.Toxic,
                    CommentModerationOrigin.Human);
            }
            else if (queue == "appeals")
            {
                result = await _projectRepository.GetCommentsWithAppealPendingAsync(page, pageSize);
                totalCount = await _projectRepository.CountCommentsWithAppealPendingAsync();
            }
            else
            {
                return Result<PaginatedResponse<ProjectCommentDto>>.Failure("Queue must be reported, ai-toxic, moderator-toxic, or appeals", "queue", ErrorType.Validation);
            }

            if (!result.IsSuccess) return Result<PaginatedResponse<ProjectCommentDto>>.Failure(result.Errors, result.ErrorType);

            var commentDtos = new List<ProjectCommentDto>();
            foreach (var comment in result.Value)
            {
                commentDtos.Add(await ToCommentDto(comment));
            }

            return Result<PaginatedResponse<ProjectCommentDto>>.Success(new PaginatedResponse<ProjectCommentDto>(
                commentDtos,
                page,
                pageSize,
                totalCount,
                (int)Math.Ceiling(totalCount / (double)pageSize)));
        }

        public async Task<Result<ProjectCommentDto>> ApproveCommentAsync(Guid commentId)
        {
            var result = await _projectRepository.GetCommentByIdAsync(commentId);
            if (!result.IsSuccess) return Result<ProjectCommentDto>.Failure(result.Errors, result.ErrorType);

            var comment = result.Value;
            comment.ModerationStatus = CommentModerationStatus.Approved;
            comment.ModeratedBy = CommentModerationOrigin.Human;
            comment.Reports.Clear();

            var updateResult = await _projectRepository.UpdateCommentAsync(comment);
            if (!updateResult.IsSuccess) return Result<ProjectCommentDto>.Failure(updateResult.Errors, updateResult.ErrorType);

            return Result<ProjectCommentDto>.Success(await ToCommentDto(comment));
        }

        public async Task<Result<ProjectCommentDto>> MarkCommentToxicAsync(Guid commentId)
        {
            var result = await _projectRepository.GetCommentByIdAsync(commentId);
            if (!result.IsSuccess) return Result<ProjectCommentDto>.Failure(result.Errors, result.ErrorType);

            var comment = result.Value;
            comment.ModerationStatus = CommentModerationStatus.Toxic;
            comment.ModeratedBy = CommentModerationOrigin.Human;

            var updateResult = await _projectRepository.UpdateCommentAsync(comment);
            if (!updateResult.IsSuccess) return Result<ProjectCommentDto>.Failure(updateResult.Errors, updateResult.ErrorType);

            return Result<ProjectCommentDto>.Success(await ToCommentDto(comment));
        }

        public async Task<Result<ProjectCommentDto>> AppealCommentAsync(Guid commentId, Guid userId, string? message)
        {
            var result = await _projectRepository.GetCommentByIdAsync(commentId);
            if (!result.IsSuccess) return Result<ProjectCommentDto>.Failure(result.Errors, result.ErrorType);

            var comment = result.Value;
            if (comment.AuthorId != userId)
                return Result<ProjectCommentDto>.Failure("You can only appeal your own comments", "authorId", ErrorType.Unauthorized);

            if (comment.ModerationStatus != CommentModerationStatus.Toxic)
                return Result<ProjectCommentDto>.Failure("Only toxic comments can be appealed", "status", ErrorType.Validation);

            if (comment.AppealStatus != CommentAppealStatus.None)
                return Result<ProjectCommentDto>.Failure("Comment already has an appeal pending or resolved", "status", ErrorType.Conflict);

            comment.AppealStatus = CommentAppealStatus.Pending;
            comment.AppealMessage = message;

            var updateResult = await _projectRepository.UpdateCommentAsync(comment);
            if (!updateResult.IsSuccess) return Result<ProjectCommentDto>.Failure(updateResult.Errors, updateResult.ErrorType);

            return Result<ProjectCommentDto>.Success(await ToCommentDto(comment));
        }

        public async Task<Result<ProjectCommentDto>> ResolveAppealAsync(Guid commentId, bool approved)
        {
            var result = await _projectRepository.GetCommentByIdAsync(commentId);
            if (!result.IsSuccess) return Result<ProjectCommentDto>.Failure(result.Errors, result.ErrorType);

            var comment = result.Value;
            if (comment.AppealStatus != CommentAppealStatus.Pending)
                return Result<ProjectCommentDto>.Failure("No pending appeal for this comment", "status", ErrorType.Validation);

            if (approved)
            {
                comment.ModerationStatus = CommentModerationStatus.Approved;
                comment.ModeratedBy = CommentModerationOrigin.Human;
                comment.AppealStatus = CommentAppealStatus.Approved;
            }
            else
            {
                comment.AppealStatus = CommentAppealStatus.Rejected;
            }

            var updateResult = await _projectRepository.UpdateCommentAsync(comment);
            if (!updateResult.IsSuccess) return Result<ProjectCommentDto>.Failure(updateResult.Errors, updateResult.ErrorType);

            return Result<ProjectCommentDto>.Success(await ToCommentDto(comment));
        }

        public async Task<Result<List<ActivityDto>>> GetUserActivityAsync(Guid userId)
        {
            var activityList = new List<ActivityDto>();

            var postsResult = await _projectRepository.GetProjectsByAuthorIdAsync(userId);
            if (postsResult.IsSuccess)
            {
                foreach (var post in postsResult.Value)
                {
                    var activity = new ActivityDto(
                        Type: "post",
                        Id: post.Id,
                        Title: post.Name,
                        Content: post.Description,
                        CreatedAt: post.CreatedAt,
                        ProjectName: null,
                        ProjectId: null
                    );
                    activityList.Add(activity);
                }
            }

            var ratingsResult = await _projectRepository.GetRatingsByAuthorIdAsync(userId);
            if (ratingsResult.IsSuccess)
            {
                foreach (var rating in ratingsResult.Value)
                {
                    var activity = new ActivityDto(
                        Type: "rating",
                        Id: rating.Id,
                        Title: $"Rated {rating.Project.Name}",
                        Content: rating.Score.ToString(),
                        CreatedAt: rating.DateTime,
                        ProjectName: rating.Project.Name,
                        ProjectId: rating.ProjectId
                    );
                    activityList.Add(activity);
                }
            }

            var commentsResult = await _projectRepository.GetCommentsByAuthorIdAsync(userId);
            if (commentsResult.IsSuccess)
            {
                foreach (var comment in commentsResult.Value)
                {
                    var activity = new ActivityDto(
                        Type: "comment",
                        Id: comment.Id,
                        Title: null,
                        Content: comment.Content,
                        CreatedAt: comment.CreatedAt,
                        ProjectName: comment.Project?.Name,
                        ProjectId: comment.ProjectId
                    );
                    activityList.Add(activity);
                }
            }

            activityList = activityList.OrderByDescending(a => a.CreatedAt).Take(10).ToList();

            return Result<List<ActivityDto>>.Success(activityList);
        }

        public async Task<Result<List<LeaderboardUserDto>>> GetLeaderboardAsync(string type, string period, int page, int pageSize)
        {
            if (type != "activity" && type != "rating")
                return Result<List<LeaderboardUserDto>>.Failure("Invalid type", "type", ErrorType.Validation);

            var validPeriods = new[] { "weekly-current", "weekly-last", "monthly-current", "monthly-last", "yearly-current" };
            if (!validPeriods.Contains(period))
                return Result<List<LeaderboardUserDto>>.Failure("Invalid period", "period", ErrorType.Validation);

            if (type == "activity")
            {
                return await _projectRepository.GetActivityLeaderboardAsync(period, page, pageSize);
            }
            else
            {
                return await _projectRepository.GetRatingLeaderboardAsync(period, page, pageSize);
            }
        }

        // --- New methods ---

        public async Task<Result<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            var categories = await _projectRepository.GetAllCategoriesAsync();
            var dtos = categories.Select(c => new CategoryDto(c.Id, c.Name)).ToList();
            return Result<List<CategoryDto>>.Success(dtos);
        }

        public async Task<Result<CategoryDto>> CreateCategoryAsync(string name)
        {
            var category = new Category { Name = name };
            var result = await _projectRepository.CreateCategoryAsync(category);
            return Result<CategoryDto>.Success(new CategoryDto(result.Id, result.Name));
        }

        public async Task<Result> DeleteCategoryAsync(Guid id)
        {
            await _projectRepository.DeleteCategoryAsync(id);
            return Result.Success();
        }

        public async Task<Result<List<TagDto>>> GetAllTagsAsync()
        {
            var tags = await _projectRepository.GetAllTagsAsync();
            var dtos = tags.Select(t => new TagDto(t.Id, t.Name)).ToList();
            return Result<List<TagDto>>.Success(dtos);
        }

        public async Task<Result<TagDto>> CreateTagAsync(string name)
        {
            var tag = new Tag { Name = name };
            var result = await _projectRepository.CreateTagAsync(tag);
            return Result<TagDto>.Success(new TagDto(result.Id, result.Name));
        }

        public async Task<Result> DeleteTagAsync(Guid id)
        {
            await _projectRepository.DeleteTagAsync(id);
            return Result.Success();
        }

        public async Task<Result<List<CriterionDto>>> GetCriteriaByCategoryIdAsync(Guid categoryId)
        {
            var criteria = await _projectRepository.GetCriteriaByCategoryIdAsync(categoryId);
            var dtos = criteria.Select(c => new CriterionDto(c.Id, c.CategoryId, c.Category?.Name ?? "", c.Name)).ToList();
            return Result<List<CriterionDto>>.Success(dtos);
        }

        public async Task<Result<CriterionDto>> CreateCriterionAsync(string name, Guid categoryId)
        {
            var category = await _projectRepository.GetCategoryByIdAsync(categoryId);
            if (category == null)
                return Result<CriterionDto>.Failure("Category not found", "categoryId", ErrorType.NotFound);

            var criterion = new Criterion { Name = name, CategoryId = categoryId };
            var result = await _projectRepository.CreateCriterionAsync(criterion);
            return Result<CriterionDto>.Success(new CriterionDto(result.Id, result.CategoryId, category.Name, result.Name));
        }

        public async Task<Result> DeleteCriterionAsync(Guid id)
        {
            await _projectRepository.DeleteCriterionAsync(id);
            return Result.Success();
        }

        public async Task<Result> SubmitCriterionScoresAsync(Guid projectId, Guid teacherId, List<(Guid CriterionId, int Score, string? Comment)> scores)
        {
            foreach (var (criterionId, score, comment) in scores)
            {
                var criterionScore = new CriterionScore
                {
                    ProjectId = projectId,
                    CriterionId = criterionId,
                    TeacherId = teacherId,
                    Score = score,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow
                };
                await _projectRepository.AddCriterionScoreAsync(criterionScore);
            }

            return Result.Success();
        }

        public async Task<Result<List<CriterionScoreDto>>> GetCriterionScoresAsync(Guid projectId)
        {
            var scores = await _projectRepository.GetCriterionScoresByProjectIdAsync(projectId);
            var dtos = scores.Select(s => new CriterionScoreDto(
                s.CriterionId,
                s.Criterion?.Name ?? "",
                s.Criterion?.Category?.Name ?? "",
                s.Score,
                s.Comment,
                s.Teacher?.FullName ?? "",
                s.CreatedAt
            )).ToList();
            return Result<List<CriterionScoreDto>>.Success(dtos);
        }

        public async Task<Result<PaginatedResponse<ProjectDto>>> GetFilteredProjectsAsync(string? search, Guid? categoryId, Guid? tagId, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (projects, totalCount) = await _projectRepository.GetFilteredProjectsAsync(search, categoryId, tagId, page, pageSize);
            var dtos = new List<ProjectDto>();

            foreach (var p in projects)
            {
                var avgRating = await GetAverageRatingAsync(p.Id);
                var categories = p.ProjectCategories?.Select(pc => new CategoryDto(pc.CategoryId, pc.Category?.Name ?? "")).ToList();
                var tags = p.ProjectTags?.Select(pt => new TagDto(pt.TagId, pt.Tag?.Name ?? "")).ToList();

                dtos.Add(new ProjectDto(
                    Id: p.Id,
                    Name: p.Name,
                    Description: p.Description,
                    Files: p.Attachments.Select(a => a.Path).ToList(),
                    AuthorName: p.Author.FullName,
                    AuthorUsername: p.Author.Username,
                    AuthorProfilePicturePath: $"api/users/by-username/{p.Author.Username}/profile-picture",
                    CreationDate: p.CreatedAt,
                    AverageRating: avgRating.IsSuccess ? avgRating.Value : null,
                    Categories: categories,
                    Tags: tags
                ));
            }

            return Result<PaginatedResponse<ProjectDto>>.Success(new PaginatedResponse<ProjectDto>(
                dtos, page, pageSize, totalCount, (int)Math.Ceiling(totalCount / (double)pageSize)));
        }
    }
}