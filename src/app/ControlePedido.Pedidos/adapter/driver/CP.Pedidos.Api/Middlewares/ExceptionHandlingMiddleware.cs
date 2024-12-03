using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using CP.Pedidos.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace CP.Pedidos.Api.Middlewares;

[ExcludeFromCodeCoverage]
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next,
                                       ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode status = HttpStatusCode.InternalServerError)
    {
        _logger.LogError(exception, "Ocorreu um erro ao processar a requisição: {Message}. StackTrace: {StackTrace}", exception.Message, exception.StackTrace);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var errorDetails = new ValidationProblemDetails(new Dictionary<string, string[]> {
                {
                    "Mensagens", new string[]{exception.Message}
                }
            });

        return context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails));
    }

}
