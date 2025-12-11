using StudentHub.Application.DTOs;

namespace StudentHub.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<Result<string>> SaveFileAsync(Stream fileStream, string fileName);
        Task<Result<Stream>> GetFileAsync(string relativePath);
        Task<Result<string>> SaveProfilePictureAsync(Stream fileStream, string fileName);
    }
}
