using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Application.Interfaces.Services;
using StudentHub.Domain.Entities;

namespace StudentHub.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IFileStorageService _fileService;
        public ProjectService(IProjectRepository projectRepository, IFileStorageService fileService)
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
            var projectDto = new ProjectDto(value.Id, value.Name, value.Description, filePaths, Author: value.Author.FullName, CreationDate: value.CreatedAt);
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

        public async Task<Result> DeleteAsync(Guid id)
        {
            var result = await _projectRepository.DeleteAsync(id);
            return result;
        }

        public async Task<List<ProjectDto>> GetAllAsync(int page = 0, int pageSize = 0)
        {
            var projectList = await _projectRepository.GetAllAsync(page, pageSize);
            var dtoList = projectList.Select(p => new ProjectDto(
                Name: p.Name,
                Description: p.Description,
                Files: p.Images.Select(i => i.Path).ToList(),
                Id: p.Id,
                Author: p.Author.FullName,
                CreationDate: p.CreatedAt
            ))
                .ToList();

            return dtoList;
        }

        public async Task<Result<ProjectDto?>> GetByIdAsync(Guid id)
        {
            var projectResult = await _projectRepository.GetByIdAsync(id);
            if (!projectResult.IsSuccess) return Result<ProjectDto?>.Failure(projectResult.Errors);
            var project = projectResult.Value;
            var projectDto = new ProjectDto(
                Id: project.Id,
                Name: project.Name,
                Description: project.Description,
                Files: project.Images.Select(i => i.Path).ToList(),
                Author: project.Author.FullName,
                CreationDate: project.CreatedAt
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

        public Task<Result<ProjectDto?>> UpdateAsync(CreateProjectCommand updateProjectCommand)
        {
            throw new NotImplementedException();
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

            // Get user's score for this project
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

            var comments = result.Value ?? new List<ProjectComment>();
            var commentDtos = new List<ProjectCommentDto>();

            foreach (var comment in comments)
            {
                // Get user's score for this project
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
    }
}
