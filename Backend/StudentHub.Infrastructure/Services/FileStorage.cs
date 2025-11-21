using StudentHub.Application.DTOs;
using StudentHub.Application.Interfaces.Services;

namespace StudentHub.Infrastructure.Services
{
    public class FileStorage : IFileStorageService
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        public FileStorage()
        {
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }
        public async Task<Result<string>> SaveFileAsync(Stream fileStream, string fileName)
        {
            var uniqueName = $"{Guid.NewGuid()}_{fileName}";
            var fullPath = Path.Combine(_basePath, uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            return Result<string>.Success(uniqueName);
        }

        public async Task<Result<Stream>> GetFileAsync(string relativePath)
        {
            var fullPath = Path.Combine(_basePath, relativePath);

            if (!File.Exists(fullPath))
                return Result<Stream>.Failure("Файл не найден");

            return Result<Stream>.Success(File.OpenRead(fullPath));
        }

    }
}
