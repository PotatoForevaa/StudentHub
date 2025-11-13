using Microsoft.AspNetCore.Mvc;
using StudentHub.Application.DTOs;

namespace StudentHub.Api.Extensions
{
    public static class ResultExtension
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);

            return CreateErrorResult(result.ErrorType, result.Error);
        }

        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
                return new OkResult();

            return CreateErrorResult(result.ErrorType, result.Error);
        }

        private static IActionResult CreateErrorResult(ErrorType type, string? message)
        {
            var problem = new ProblemDetails
            {
                Title = GetTitle(type),
                Status = GetStatusCode(type),
                Detail = message
            };

            return new ObjectResult(problem)
            {   
                StatusCode = problem.Status
            };
        }

        private static string GetTitle(ErrorType type) => type switch
        {
            ErrorType.NotFound => "Not Found",
            ErrorType.Validation => "Validation Error",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.Conflict => "Conflict",
            ErrorType.ServerError => "Server Error",
            _ => "Unknown Error"
        };

        private static int GetStatusCode(ErrorType type) => type switch
        {
            ErrorType.NotFound => 404,
            ErrorType.Validation => 400,
            ErrorType.Unauthorized => 401,
            ErrorType.Conflict => 409,
            ErrorType.ServerError => 500,
            _ => 500
        };
    }
}
