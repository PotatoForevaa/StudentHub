using Microsoft.AspNetCore.Mvc;
using StudentHub.Application.DTOs;

namespace StudentHub.Api.Extensions
{
    public static class ResultExtension
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => new NotFoundObjectResult(result.Error),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(result.Error),
                ErrorType.Validation => new BadRequestObjectResult(result.Error),
                _ => new StatusCodeResult(500)
            };
        }

        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess) return new OkResult();

            return result.ErrorType switch
            {
                ErrorType.NotFound => new NotFoundObjectResult(result.Error),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(result.Error),
                ErrorType.Validation => new BadRequestObjectResult(result.Error),
                _ => new StatusCodeResult(500)
            };
        }
    }
}
