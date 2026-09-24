using System.Net;
using System.Text.Json;

namespace Gym.Middleware;

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
            _logger.LogError(ex, "Excepción no controlada");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new { mensaje = exception.Message };

        return exception switch
        {
            KeyNotFoundException => 
                HandleResponse(context, HttpStatusCode.NotFound, response),
            
            InvalidOperationException => 
                HandleResponse(context, HttpStatusCode.BadRequest, response),
            
            _ => HandleResponse(context, HttpStatusCode.InternalServerError, response)
        };
    }

    private static Task HandleResponse(HttpContext context, HttpStatusCode statusCode, object response)
    {
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsJsonAsync(response);
    }
}