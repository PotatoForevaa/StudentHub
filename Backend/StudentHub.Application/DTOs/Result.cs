namespace StudentHub.Application.DTOs
{
    public enum ErrorType { NotFound, Validation, Unauthorized, Conflict, ServerError }

    public class Result
    {
        public bool IsSuccess => Errors.Count == 0;
        public ErrorType ErrorType { get; set; }
        public List<Error> Errors { get; set; } = new();

        public static Result Success() => new Result();

        public static Result Failure(string message, string? field = null, ErrorType errorType = ErrorType.ServerError)
            => new Result
            {
                ErrorType = errorType,
                Errors = { new Error { Message = message, Field = field } }
            };

        public static Result Failure(List<Error> errors, ErrorType errorType = ErrorType.ServerError)
            => new Result
            {
                ErrorType = errorType,
                Errors = errors
            };
    }

    public class Result<T> : Result
    {
        public T? Value { get; set; }

        public static Result<T> Success(T value) =>
            new Result<T> { Value = value };

        public static Result<T> Failure(string message, string? field = null, ErrorType errorType = ErrorType.ServerError) =>
            new Result<T>
            {
                ErrorType = errorType,
                Errors = { new Error { Message = message, Field = field } }
            };

        public static Result<T> Failure(List<Error> errors, ErrorType errorType = ErrorType.ServerError) =>
            new Result<T>
            {
                ErrorType = errorType,
                Errors = errors
            };
    }
}
