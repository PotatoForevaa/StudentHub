using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Commands;
using StudentHub.Application.DTOs.Requests;
using StudentHub.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHub.Application.Interfaces.Services
{
    public interface IProjectService
    {
        Task<Result<ProjectDto?>> GetByIdAsync(Guid id);
        Task<List<ProjectDto>> GetAllAsync(int page = 0, int pagesize = 0);
        Task<Result<ProjectDto?>> CreateAsync(CreateProjectCommand createProjectCommand);
        Task<Result<ProjectDto?>> UpdateAsync(CreateProjectCommand updateProjectCommand);
        Task<Result> DeleteAsync(Guid id);
    }
}
