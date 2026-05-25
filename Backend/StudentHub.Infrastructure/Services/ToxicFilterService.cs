using System.Net.Http;
using System.Text;
using System.Text.Json;
using StudentHub.Application.DTOs;
using StudentHub.Application.DTOs.Responses;
using StudentHub.Application.Interfaces.Services;

namespace StudentHub.Infrastructure.Services
{
    public class ToxicFilterService : IToxicFilterService
    {
        private readonly HttpClient _httpClient;

        public ToxicFilterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<ToxicFilterResultDto>> PredictAsync(string text)
        {
            try
            {
                var request = new { text };
                var json = JsonSerializer.Serialize(request);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync("predict", content);

                if (!response.IsSuccessStatusCode)
                {
                    return Result<ToxicFilterResultDto>.Failure(
                        $"Toxic filter request failed with status {(int)response.StatusCode}.",
                        errorType: ErrorType.ServerError);
                }

                await using var stream = await response.Content.ReadAsStreamAsync();
                using var document = await JsonDocument.ParseAsync(stream);
                var root = document.RootElement;

                var prediction = root.GetProperty("prediction").GetString() ?? string.Empty;
                var toxicProbability = root.TryGetProperty("toxic_probability", out var probabilityElement)
                    ? (float)probabilityElement.GetDouble()
                    : 0f;

                var isToxic = string.Equals(prediction, "toxic", StringComparison.OrdinalIgnoreCase);

                return Result<ToxicFilterResultDto>.Success(new ToxicFilterResultDto(isToxic, toxicProbability, prediction));
            }
            catch (Exception ex)
            {
                return Result<ToxicFilterResultDto>.Failure(
                    $"Toxic filter request failed: {ex.Message}",
                    errorType: ErrorType.ServerError);
            }
        }

        public async Task<Result<string>> PredictAsync(string text, Guid commentId)
        {
            try
            {
                var request = new { text, comment_id = commentId.ToString() };
                var json = JsonSerializer.Serialize(request);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync("predict_async_linked", content);

                if (!response.IsSuccessStatusCode)
                {
                    return Result<string>.Failure(
                        $"Toxic filter async request failed with status {(int)response.StatusCode}.",
                        errorType: ErrorType.ServerError);
                }

                await using var stream = await response.Content.ReadAsStreamAsync();
                using var document = await JsonDocument.ParseAsync(stream);
                var root = document.RootElement;

                var taskId = root.GetProperty("task_id").GetString();
                if (string.IsNullOrWhiteSpace(taskId))
                {
                    return Result<string>.Failure("Toxic filter async request did not return a task_id.", errorType: ErrorType.ServerError);
                }

                return Result<string>.Success(taskId);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(
                    $"Toxic filter async request failed: {ex.Message}",
                    errorType: ErrorType.ServerError);
            }
        }

        public async Task<Result<ToxicFilterTaskResultDto>> GetTaskResultAsync(string taskId)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"task_result/{taskId}");
                if (!response.IsSuccessStatusCode)
                {
                    return Result<ToxicFilterTaskResultDto>.Failure(
                        $"Toxic filter task result request failed with status {(int)response.StatusCode}.",
                        errorType: ErrorType.ServerError);
                }

                await using var stream = await response.Content.ReadAsStreamAsync();
                using var document = await JsonDocument.ParseAsync(stream);
                var root = document.RootElement;
                var status = root.GetProperty("status").GetString() ?? string.Empty;

                if (string.Equals(status, "pending", StringComparison.OrdinalIgnoreCase))
                {
                    return Result<ToxicFilterTaskResultDto>.Success(new ToxicFilterTaskResultDto(status, false, 0f, string.Empty));
                }

                if (string.Equals(status, "failure", StringComparison.OrdinalIgnoreCase))
                {
                    var error = root.TryGetProperty("error", out var errorElement) ? errorElement.GetString() : "Unknown error";
                    return Result<ToxicFilterTaskResultDto>.Success(new ToxicFilterTaskResultDto(status, false, 0f, string.Empty, error));
                }

                var resultElement = root.GetProperty("result");
                var modelResultElement = resultElement;
                if (!modelResultElement.TryGetProperty("prediction", out _) &&
                    resultElement.TryGetProperty("result", out var nestedResultElement))
                {
                    modelResultElement = nestedResultElement;
                }

                var prediction = modelResultElement.GetProperty("prediction").GetString() ?? string.Empty;
                var toxicProbability = modelResultElement.TryGetProperty("toxic_probability", out var probabilityElement)
                    ? (float)probabilityElement.GetDouble()
                    : 0f;
                var isToxic = string.Equals(prediction, "toxic", StringComparison.OrdinalIgnoreCase);

                return Result<ToxicFilterTaskResultDto>.Success(new ToxicFilterTaskResultDto(status, isToxic, toxicProbability, prediction));
            }
            catch (Exception ex)
            {
                return Result<ToxicFilterTaskResultDto>.Failure(
                    $"Toxic filter task result request failed: {ex.Message}",
                    errorType: ErrorType.ServerError);
            }
        }
    }
}
