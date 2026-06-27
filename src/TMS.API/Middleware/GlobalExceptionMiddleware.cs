using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TMS.Domain.Exceptions;
using TMS.Shared;

namespace TMS.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next   = next;
        _logger = logger;
        _env    = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        ApiResponse<object> response;

        switch (ex)
        {
            case NotFoundException nfe:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = ApiResponse<object>.Fail(nfe.Message, 404);
                break;

            case ValidationException ve:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var errors = ve.Errors.Select(e => e.ErrorMessage).ToList();
                response = ApiResponse<object>.Fail(errors, 400);
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = ApiResponse<object>.Fail("Unauthorized.", 401);
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var message = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred.";
                response = ApiResponse<object>.Fail(message, 500);
                break;
        }

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}
