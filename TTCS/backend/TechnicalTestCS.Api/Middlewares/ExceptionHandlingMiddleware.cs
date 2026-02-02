using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TechnicalTestCS.Api.Services;
using TechnicalTestCS.Infrastructure.External;

namespace TechnicalTestCS.Api.Middlewares
{
    public sealed class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (InvalidTransitionException ex)
            {
                _logger.LogInformation(ex, "Invalid status transition.");
                await WriteProblem(context, StatusCodes.Status400BadRequest,
                    title: "Invalid transition",
                    detail: ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogInformation(ex, "Bad request.");
                await WriteProblem(context, StatusCodes.Status400BadRequest,
                    title: "Invalid request",
                    detail: ex.Message);
            }
            catch (ExternalApiException ex)
            {
                // CMS failure
                _logger.LogWarning(ex, "External API error (status {StatusCode}).", ex.StatusCode);
                await WriteProblem(context, StatusCodes.Status502BadGateway,
                    title: "External API error",
                    detail: ex.Message);
            }
            catch (HttpRequestException ex)
            {
                // DNS etc 
                _logger.LogWarning(ex, "External API unreachable.");
                await WriteProblem(context, StatusCodes.Status502BadGateway,
                    title: "External API unreachable",
                    detail: ex.Message);
            }
            catch (TaskCanceledException ex) when (!context.RequestAborted.IsCancellationRequested)
            {
                // Timeout HttpClient 
                _logger.LogWarning(ex, "External API timeout.");
                await WriteProblem(context, StatusCodes.Status504GatewayTimeout,
                    title: "External API timeout",
                    detail: "External API did not respond in time.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception.");
                await WriteProblem(context, StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error",
                    detail: "An unexpected error occurred.");
            }
        }

        private static async Task WriteProblem(HttpContext context, int statusCode, string title, string detail)
        {
            if (context.Response.HasStarted)
                return;

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
