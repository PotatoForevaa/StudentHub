namespace StudentHub.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveImageAsync(string base64image);
    }
}
