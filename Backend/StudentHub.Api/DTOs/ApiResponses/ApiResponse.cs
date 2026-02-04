namespace StudentHub.Api.DTOs.Responses
{
    /// <typeparam name="T">The data type being returned</typeparam>
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public List<ApiError> Errors { get; set; } = new();
        public string? ErrorType { get; set; }
    }

    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public List<ApiError> Errors { get; set; } = new();
        public string? ErrorType { get; set; }
    }

    public class ApiError
    {
        public string Message { get; set; } = string.Empty;
        public string? Field { get; set; }
    }
}
