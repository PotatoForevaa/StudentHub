namespace StudentHub.Application.DTOs
{
    public enum ErrorType { NotFound, Validation, Unauthorized, Conflict, ServerError }

    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
        public ErrorType ErrorType { get; set; }
        protected Result(bool isSuccess, string? error, ErrorType errorType)
        {
            IsSuccess = isSuccess;
            Error = error;
            ErrorType = errorType;
        }

        public static Result Success() => new Result(true, null, default);
        public static Result Failure(string? error, ErrorType errorType = ErrorType.ServerError)
            => new Result(false, error, errorType);

    }

    public class Result<TValue> : Result
    {
        public TValue? Value { get; set; }

        private Result(TValue value) : base(true, null, default)
        {
            Value = value;
        }

        private Result(string? error, ErrorType errorType) : base(false, error, errorType)
        {
            Value = default;
        }

        public static Result<TValue> Success(TValue value) => new Result<TValue>(value);

        public static Result<TValue> Failure(string? error, ErrorType errorType = ErrorType.ServerError)
            => new Result<TValue>(error, errorType);
    }
}
