using StudentHub.Application.Interfaces.Services;

namespace StudentHub.Infrastructure.Services
{
    public class FileStorage : IFileStorageService
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        public async Task<List<string>> SaveImagesAsync(List<string> base64images)
        {
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);

            var filePaths = new List<string>();
            foreach (var image in base64images)
            {
                var bytes = Convert.FromBase64String(image);
                var fileName = $"{Guid.NewGuid()}.png";
                var fullPath = Path.Combine(_basePath, fileName);
                filePaths.Add(fullPath);
                await File.WriteAllBytesAsync(fullPath, bytes);
            }

            return filePaths;
        }
    }
}
