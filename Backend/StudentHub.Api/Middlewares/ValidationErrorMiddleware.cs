using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace StudentHub.Api.Middlewares
{
    public class ValidationErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBody = context.Response.Body;
            using var mem = new MemoryStream();
            context.Response.Body = mem;

            await _next(context);

            if (context.Response.StatusCode != StatusCodes.Status400BadRequest)
            {
                mem.Position = 0;
                await mem.CopyToAsync(originalBody);
                return;
            }

            mem.Position = 0;
            var bodyText = await new StreamReader(mem).ReadToEndAsync();

            if (!string.IsNullOrWhiteSpace(bodyText) &&
                bodyText.Contains("\"errors\":"))
            {

                var problem = JsonSerializer.Deserialize<ValidationProblemDetails>(bodyText);

                var normalized = problem.Errors
                    .SelectMany(kvp => kvp.Value.Select(msg => new
                    {
                        field = kvp.Key,
                        message = msg
                    }));

                var payload = JsonSerializer.Serialize(new { errors = normalized });

                context.Response.Body = originalBody;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(payload);
            }
            else
            {
                context.Response.Body = originalBody;
                await context.Response.WriteAsync(bodyText);
            }
        }
    }

}
