using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Application.Interfaces.Services;
using StudentHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var projectDto = new ProjectDto(value.Id, value.Name, value.Description, filePaths);
            return Result<ProjectDto?>.Success(projectDto); ;
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var result = await _projectRepository.DeleteAsync(id);
            return result;
        }

        public Task<List<ProjectDto>> GetAllAsync(int page = 0, int pagesize = 0)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ProjectDto?>> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ProjectDto?>> UpdateAsync(CreateProjectCommand updateProjectCommand)
        {
            throw new NotImplementedException();
        }
    }
}
