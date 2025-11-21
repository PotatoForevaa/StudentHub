using StudentHub.Application.DTOs;

namespace StudentHub.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<Result<string>> SaveFileAsync(Stream fileStream, string fileName);
        Task<Result<Stream>> GetFileAsync(string relativePath);
    }
}
