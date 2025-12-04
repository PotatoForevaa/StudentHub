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
            return Result<ProjectDto?>.Success(projectDto); ;
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
    }
}
