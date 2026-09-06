using System.Net;
using System.Text.Json;
using JobTracker.Application.Exceptions;

namespace JobTracker.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.NotFound,
                    ex.Message
                    );
            }
            catch (ConflictException ex)
            {
                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.Conflict,
                    ex.Message
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");

                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred."
                    );
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string message)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode = (int)statusCode,
                message = message
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
