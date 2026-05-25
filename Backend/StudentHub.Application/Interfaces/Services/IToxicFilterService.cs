using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Responses;

namespace StudentHub.Application.Interfaces.Services
{
    public interface IToxicFilterService
    {
        Task<Result<ToxicFilterResultDto>> PredictAsync(string text);
        Task<Result<string>> PredictAsync(string text, Guid commentId);
        Task<Result<ToxicFilterTaskResultDto>> GetTaskResultAsync(string taskId);
    }
}
