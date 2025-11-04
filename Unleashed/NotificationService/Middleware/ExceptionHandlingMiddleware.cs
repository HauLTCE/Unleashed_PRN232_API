using NotificationService.Exceptions;
using System.Net;
using System.Text.Json;

namespace NotificationService.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred.");

            HttpStatusCode statusCode;
            string message;

            switch (exception)
            {
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundException.Message;
                    break;
                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = badRequestException.Message;
                    break;
                case ForbiddenException forbiddenException:
                    statusCode = HttpStatusCode.Forbidden;
                    message = forbiddenException.Message;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "An internal server error has occurred.";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new { error = message };
            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
