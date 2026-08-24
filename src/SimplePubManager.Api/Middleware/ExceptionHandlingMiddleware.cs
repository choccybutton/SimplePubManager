using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimplePubManager.Shared.Dto;

namespace SimplePubManager.Api.Middleware
{
    /// <summary>
    /// Global exception handling middleware for catching and formatting unhandled exceptions.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the ExceptionHandlingMiddleware class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="logger">The logger instance</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Invokes the middleware to handle the HTTP request and catch any exceptions.
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An unhandled exception occurred while processing the request.");
                await HandleExceptionAsync(context, exception);
            }
        }

        /// <summary>
        /// Handles an exception and returns a formatted error response.
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <param name="exception">The exception to handle</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Determine the HTTP status code based on exception type
            var statusCode = exception switch
            {
                ArgumentNullException => HttpStatusCode.BadRequest,
                ArgumentException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            // Create a consistent error response
            var response = new ApiResponse<object>
            {
                Error = new ApiError
                {
                    Code = statusCode.ToString(),
                    Message = GetErrorMessage(exception),
                    Details = new Dictionary<string, object>
                    {
                        { "ExceptionType", exception.GetType().Name }
                    }
                }
            };

            // Include stack trace in development environment only
            if (context.RequestServices.GetService(typeof(IHostEnvironment))
                is IHostEnvironment hostEnv
                && hostEnv.IsDevelopment())
            {
                response.Error.Details?.Add("StackTrace", exception.StackTrace ?? "No stack trace available");
            }

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        /// <summary>
        /// Gets a user-friendly error message from the exception.
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <returns>A user-friendly error message</returns>
        private static string GetErrorMessage(Exception exception)
        {
            return exception switch
            {
                ArgumentNullException => "A required parameter is missing.",
                ArgumentException => "An invalid argument was provided.",
                InvalidOperationException => "The operation could not be completed.",
                UnauthorizedAccessException => "You are not authorized to perform this action.",
                _ => "An unexpected error occurred while processing your request."
            };
        }
    }
}
