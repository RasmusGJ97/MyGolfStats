using Microsoft.AspNetCore.Http.Features;
using MyGolfStatsApi.Db.AppDbContext;
using MyGolfStatsApi.Db.Models;
using System.Diagnostics;
using System.Text.Json;

namespace MyGolfStatsApi
{
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiLoggingMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ApiLoggingMiddleware(RequestDelegate next, ILogger<ApiLoggingMiddleware> logger, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var startTime = DateTime.UtcNow;

            int statusCode = 200;
            string? errorMessage = null;
            string? stackTrace = null;

            try
            {
                await _next(context);
                statusCode = context.Response.StatusCode;
            }
            catch (Exception ex)
            {
                statusCode = 500;
                errorMessage = ex.Message;
                stackTrace = ex.StackTrace;

                _logger.LogError(ex, "Unhandled exception caught in ApiLoggingMiddleware");

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var errorResponse = JsonSerializer.Serialize(new { error = "Ett oväntat fel inträffade" });
                await context.Response.WriteAsync(errorResponse);
            }
            finally
            {
                stopwatch.Stop();

                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var log = new ApiLogEntry
                {
                    Timestamp = startTime,
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    QueryString = context.Request.QueryString.ToString(),
                    StatusCode = statusCode,
                    StatusMessage = GetStatusMessage(statusCode),
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                    UserId = context.User.Identity?.IsAuthenticated == true
                        ? context.User.Identity.Name
                        : null,
                    UserAgent = context.Request.Headers["User-Agent"].ToString(),
                    ErrorMessage = errorMessage,
                    StackTrace = stackTrace
                };

                dbContext.ApiLogEntries.Add(log);
                await dbContext.SaveChangesAsync();
            }
        }

        private static string GetStatusMessage(int statusCode)
        {
            return statusCode switch
            {
                200 => "OK",
                201 => "Created",
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => "Other"
            };
        }
    }
}
