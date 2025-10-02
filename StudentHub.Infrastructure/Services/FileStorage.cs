using StudentHub.Application.Interfaces;

namespace StudentHub.Infrastructure.Services
{
    public class FileStorage : IFileStorageService
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        public async Task<string> SaveImageAsync(string base64image)
        {
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);

            var bytes = Convert.FromBase64String(base64image);
            var fileName = $"{Guid.NewGuid()}.png";
            var fullPath = Path.Combine(_basePath, fileName);

            await File.WriteAllBytesAsync(fullPath, bytes);

            return fullPath;
        }
    }
}
