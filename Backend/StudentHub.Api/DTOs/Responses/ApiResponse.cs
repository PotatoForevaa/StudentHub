namespace StudentHub.Api.DTOs.Responses
{
    /// <summary>
    /// Standard API response wrapper for all endpoints.
    /// </summary>
    /// <typeparam name="T">The data type being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The response data (null if operation failed).
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// List of errors if the operation failed.
        /// </summary>
        public List<ApiError> Errors { get; set; } = new();

        /// <summary>
        /// Error type classification (NotFound, Validation, Unauthorized, Conflict, ServerError).
        /// </summary>
        public string? ErrorType { get; set; }
    }

    /// <summary>
    /// Non-generic response for operations that don't return data.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Indicates if the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// List of errors if the operation failed.
        /// </summary>
        public List<ApiError> Errors { get; set; } = new();

        /// <summary>
        /// Error type classification.
        /// </summary>
        public string? ErrorType { get; set; }
    }

    /// <summary>
    /// Error details in API response.
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Field name related to the error (for validation errors).
        /// </summary>
        public string? Field { get; set; }
    }
}
