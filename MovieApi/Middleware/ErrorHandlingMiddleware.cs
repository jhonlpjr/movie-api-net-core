using Application.Exceptions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;

namespace MovieApi.Middleware;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            // Log estructurado
            _logger.LogError(ex, "Unhandled exception processing {Method} {Path}", ctx.Request.Method, ctx.Request.Path);

            var (status, title) = ex switch
            {
                NotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
                ConflictException => (HttpStatusCode.Conflict, "Conflict"),
                DomainException => (HttpStatusCode.UnprocessableEntity, "Domain rule violated"),
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid argument"),
                RedisConnectionException => (HttpStatusCode.RequestTimeout, "Redis fail connection"),
                _ => (HttpStatusCode.InternalServerError, "Unexpected error")
            };

            var details = new ProblemDetails
            {
                Title = title,
                Detail = ex.Message,
                Status = (int)status,
                Instance = $"{ctx.Request.Method} {ctx.Request.Path}"
            };

            ctx.Response.ContentType = "application/problem+json";
            ctx.Response.StatusCode = details.Status ?? 500;
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(details));
        }
    }
}
