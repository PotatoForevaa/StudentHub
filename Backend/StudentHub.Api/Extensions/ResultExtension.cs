using Microsoft.AspNetCore.Mvc;
using StudentHub.Api.DTOs.Responses;
using StudentHub.Application.DTOs;

namespace StudentHub.Api.Extensions
{
    public static class ResultExtension
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                var apiResponse = new ApiResponse<T>
                {
                    IsSuccess = true,
                    Data = result.Value
                };
                return new OkObjectResult(apiResponse);
            }

            var errorType = result.ErrorType;

            return CreateErrorResult(errorType, result.Errors);
        }

        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
            {
                var apiResponse = new ApiResponse { IsSuccess = true };
                return new OkObjectResult(apiResponse);
            }

            var errorType = result.ErrorType;

            return CreateErrorResult(errorType, result.Errors);
        }

        private static IActionResult CreateErrorResult(ErrorType type, List<Error> errors)
        {
            var apiErrors = errors.Select(e => new ApiError { Message = e.Message, Field = e.Field }).ToList();

            var apiResponse = new ApiResponse
            {
                IsSuccess = false,
                Errors = apiErrors,
                ErrorType = type.ToString()
            };

            return new ObjectResult(apiResponse)
            {
                StatusCode = GetStatusCode(type)
            };
        }

        private static int GetStatusCode(ErrorType type) => type switch
        {
            ErrorType.NotFound => 404,
            ErrorType.Validation => 400,
            ErrorType.Unauthorized => 401,
            ErrorType.Conflict => 409,
            ErrorType.ServerError => 500,
            _ => 500
        };

        public static IActionResult ToCreatedActionResult<T>(this Result<T> result, string relativeUri)
        {
            if (result.IsSuccess)
            {
                var apiResponse = new ApiResponse<T>
                {
                    IsSuccess = true,
                    Data = result.Value
                };
                return new CreatedResult(relativeUri, apiResponse);
            }

            var errorType = result.ErrorType;
            return CreateErrorResult(errorType, result.Errors);
        }

        public static IActionResult ToCreatedActionResult(this Result result, string relativeUri)
        {
            if (result.IsSuccess)
            {
                var apiResponse = new ApiResponse { IsSuccess = true };
                return new CreatedResult(relativeUri, apiResponse);
            }

            var errorType = result.ErrorType;
            return CreateErrorResult(errorType, result.Errors);
        }
    }
}
