using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Entities;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Application.Interfaces.UseCases;

namespace StudentHub.Application.UseCases
{
    public class ProjectUseCase : IProjectUseCase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IFileStorageService _fileService;
        public ProjectUseCase(IProjectRepository projectRepository, IFileStorageService fileService)
        {
            _projectRepository = projectRepository;
            _fileService = fileService;
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
                Images = filePaths.Select(fp => new Image { Path = fp }).ToList()
            };

            var result = await _projectRepository.AddAsync(project);
            if (!result.IsSuccess) return Result<ProjectDto?>.Failure(result.Errors);
            var value = result.Value;
            var projectDto = new ProjectDto(value.Id, value.Name, value.Description, filePaths, AuthorName: value.Author.FullName, AuthorUsername: value.Author.Username, CreationDate: value.CreatedAt);
            return Result<ProjectDto?>.Success(projectDto);
        }

        public async Task<Result<double>> AddScoreAsync(AddProjectScoreCommand command)
        {
            if (command.Score < 1 || command.Score > 5)
                return Result<double>.Failure("Score must be between 1 and 5", "score", ErrorType.Validation);

            var rating = new ProjectRating
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

                var dto = new ProjectDto(
                    Name: p.Name,
                    Description: p.Description,
                    Files: p.Images.Select(i => i.Path).ToList(),
                    Id: p.Id,
                    AuthorName: p.Author.FullName,
                    AuthorUsername: p.Author.Username,
                    CreationDate: p.CreatedAt,
                    AverageRating: avgRating
                );
                dtoList.Add(dto);
            }

            return Result<List<ProjectDto>>.Success(dtoList);
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

                var dto = new ProjectDto(
                    Name: p.Name,
                    Description: p.Description,
                    Files: p.Images.Select(i => i.Path).ToList(),
                    Id: p.Id,
                    AuthorName: p.Author.FullName,
                    AuthorUsername: p.Author.Username,
                    CreationDate: p.CreatedAt,
                    AverageRating: avgRating
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

            var projectDto = new ProjectDto(
                Id: project.Id,
                Name: project.Name,
                Description: project.Description,
                Files: project.Images.Select(i => i.Path).ToList(),
                AuthorName: project.Author.FullName,
                AuthorUsername: project.Author.Username,
                CreationDate: project.CreatedAt,
                AverageRating: avgRating,
                Comments: comments
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

            project.Images.Clear();

            foreach (var path in filePaths)
            {
                project.Images.Add(new Image { Path = path, Project = project });
            }


            var updateResult = await _projectRepository.UpdateAsync(project);
            if (!updateResult.IsSuccess) return Result<ProjectDto?>.Failure(updateResult.Errors);

            return Result<ProjectDto?>.Success(new ProjectDto(
                Id: project.Id,
                Name: project.Name,
                Description: project.Description,
                Files: project.Images.Select(i => i.Path).ToList(),
                AuthorName: project.Author.FullName,
                AuthorUsername: project.Author.Username,
                CreationDate: project.CreatedAt
            ));
        }

        public async Task<Result<ProjectCommentDto>> AddCommentAsync(CreateProjectCommentCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Content))
                return Result<ProjectCommentDto>.Failure("Comment content cannot be empty", "content", ErrorType.Validation);

            var comment = new ProjectComment
            {
                ProjectId = command.ProjectId,
                AuthorId = command.AuthorId,
                Content = command.Content,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _projectRepository.AddCommentAsync(comment);
            if (!result.IsSuccess) return Result<ProjectCommentDto>.Failure(result.Errors);

            var commentEntity = result.Value;

            var userScore = await _projectRepository.GetUserScoreForProjectAsync(command.AuthorId, command.ProjectId);

            if (commentEntity.Author == null)
                return Result<ProjectCommentDto>.Failure("Failed to load comment author", "author", ErrorType.NotFound);

            var commentDto = new ProjectCommentDto(
                Id: commentEntity.Id,
                AuthorId: commentEntity.AuthorId,
                AuthorUsername: commentEntity.Author.Username,
                AuthorFullName: commentEntity.Author.FullName,
                AuthorProfilePicturePath: string.IsNullOrEmpty(commentEntity.Author.ProfilePicturePath) ? null : commentEntity.Author.ProfilePicturePath,
                Content: commentEntity.Content,
                CreatedAt: commentEntity.CreatedAt,
                UserScore: userScore
            );

            return Result<ProjectCommentDto>.Success(commentDto);
        }

        public async Task<Result<List<ProjectCommentDto>>> GetCommentsByProjectIdAsync(Guid projectId)
        {
            var result = await _projectRepository.GetCommentsByProjectIdAsync(projectId);
            if (!result.IsSuccess) return Result<List<ProjectCommentDto>>.Failure(result.Errors);

            var comments = result.Value;
            var commentDtos = new List<ProjectCommentDto>();

            foreach (var comment in comments)
            {
                var userScore = await _projectRepository.GetUserScoreForProjectAsync(comment.AuthorId, projectId);

                var commentDto = new ProjectCommentDto(
                    Id: comment.Id,
                    AuthorId: comment.AuthorId,
                    AuthorUsername: comment.Author.Username,
                    AuthorFullName: comment.Author.FullName,
                    AuthorProfilePicturePath: string.IsNullOrEmpty(comment.Author.ProfilePicturePath) ? null : comment.Author.ProfilePicturePath,
                    Content: comment.Content,
                    CreatedAt: comment.CreatedAt,
                    UserScore: userScore
                );

                commentDtos.Add(commentDto);
            }

            return Result<List<ProjectCommentDto>>.Success(commentDtos);
        }

        public async Task<Result<List<ProjectCommentDto>>> GetCommentsByAuthorIdAsync(Guid authorId)
        {
            var result = await _projectRepository.GetCommentsByAuthorIdAsync(authorId);
            if (!result.IsSuccess) return Result<List<ProjectCommentDto>>.Failure(result.Errors);

            var comments = result.Value;
            var commentDtos = new List<ProjectCommentDto>();

            foreach (var comment in comments)
            {
                var userScore = await _projectRepository.GetUserScoreForProjectAsync(comment.AuthorId, comment.ProjectId);

                var commentDto = new ProjectCommentDto(
                    Id: comment.Id,
                    AuthorId: comment.AuthorId,
                    AuthorUsername: comment.Author.Username,
                    AuthorFullName: comment.Author.FullName,
                    AuthorProfilePicturePath: string.IsNullOrEmpty(comment.Author.ProfilePicturePath) ? null : comment.Author.ProfilePicturePath,
                    Content: comment.Content,
                    CreatedAt: comment.CreatedAt,
                    UserScore: userScore
                );

                commentDtos.Add(commentDto);
            }

            return Result<List<ProjectCommentDto>>.Success(commentDtos);
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
    }
}
