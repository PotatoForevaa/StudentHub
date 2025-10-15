using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHub.Application.DTOs.Responses
{
    public enum ErrorType { NotFound, Validation, Unauthorized, Conflict, ServerError }
    public class Result<TValue, TError>
    {
        public TValue? Value { get; set; }
        public TError? Error { get; set; }
        public ErrorType ErrorType { get; set; }
        public bool IsSuccess { get; set; }

        private Result(TValue value)
        {
            Value = value;
            IsSuccess = true;
        }

        private Result(TError error, ErrorType? errorType = null)
        {
            Error = error;
            IsSuccess = false;
            ErrorType = ErrorType;
        }

        public static Result<TValue, TError> Success(TValue value) => new Result<TValue, TError>(value);
        public static Result<TValue, TError> Failure(TError error, ErrorType? errorType = null) => new Result<TValue, TError>(error);

        public static implicit operator Result<TValue, TError>(TValue value) => Success(value);
        public static implicit operator Result<TValue, TError>(TError error) => Failure(error);
    }
}
