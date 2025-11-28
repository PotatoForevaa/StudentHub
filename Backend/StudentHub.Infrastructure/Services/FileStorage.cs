using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp;
using StudentHub.Application.DTOs;
using StudentHub.Application.Interfaces.Services;
using SixLabors.ImageSharp.Processing;

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

        public async Task<Result<string>> SaveProfilePictureAsync(Stream fileStream, string fileName)
        {
            var uniqueName = $"{Guid.NewGuid()}_{fileName}.png";
            var fullPath = Path.Combine(_basePath, uniqueName);
            var image = await ProcessImage(fileStream);

            File.WriteAllBytes(fullPath, image);

            return Result<string>.Success(uniqueName);
        }
        
        public async Task<Result<string>> SaveFileAsync(Stream fileStream, string fileName)
        {
            var uniqueName = $"{Guid.NewGuid()}_{fileName}.png";
            var fullPath = Path.Combine(_basePath, uniqueName);

            using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite);
            await fileStream.CopyToAsync(stream);
            return Result<string>.Success(uniqueName);
        }

        public Result<Stream> GetFileAsync(string relativePath)
        {
            var fullPath = Path.Combine(_basePath, relativePath);

            if (!File.Exists(fullPath))
                return Result<Stream>.Failure("Файл не найден", null, ErrorType.NotFound);

            return Result<Stream>.Success(File.OpenRead(fullPath));
        }
        
        private async Task<byte[]> ProcessImage(Stream fileStream)
        {
            using var image = await Image.LoadAsync(fileStream);

            int size = Math.Min(image.Width, image.Height);
            int finalSize = 512;

            int x = (image.Width - size) / 2;
            int y = (image.Height - size) / 2;

            image.Mutate(i => i.Crop(new Rectangle(x, y, size, size))); //вырезаем квадрат из центра
            image.Mutate(i => i.Resize(finalSize, finalSize));

            using var ms = new MemoryStream();
            await image.SaveAsPngAsync(ms);
            return ms.ToArray();
        }
    }
}
