using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Services.Exceptions;

namespace MusicStore.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var (status, title, logLevel) = ex switch
            {
                NotFoundException => (StatusCodes.Status400BadRequest, "No se encontró el registro", LogLevel.Information),
                BusinessException => (StatusCodes.Status400BadRequest, "Regla de negocio violada", LogLevel.Information),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado", LogLevel.Warning),
                ArgumentException => (StatusCodes.Status400BadRequest, "Argumentos no válidos", LogLevel.Warning),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "No autorizado", LogLevel.Warning),
                AutoMapperMappingException => (StatusCodes.Status400BadRequest, "Argumentos no válidos", LogLevel.Warning),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor", LogLevel.Error)
            };

            logger.Log(logLevel, ex, "Error manejado por middleware");

            var detail = ex switch
            {
                NotFoundException => ex.Message,
                BusinessException => ex.Message,
                KeyNotFoundException => ex.Message,
                ArgumentException => ex.Message,
                UnauthorizedAccessException => ex.Message,
                AutoMapperMappingException => ex.Message,
                _ => "Ocurrió un error inesperado"
            };

            await WriteProblemDetails(context, status, title, detail);
        }
    }

    private static async Task WriteProblemDetails(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}