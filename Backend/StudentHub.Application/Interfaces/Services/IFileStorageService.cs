namespace StudentHub.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<List<string>> SaveImagesAsync(List<string> base64images);
    }
}
