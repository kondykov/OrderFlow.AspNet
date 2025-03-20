using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models;

namespace OrderFlow.Shared.Filters;

public class ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        OperationResult? response;
        var statusCode = 400;

        switch (true)
        {
            case { } when exception is DuplicatedEntityException:
            {
                statusCode = 409;
                response = new OperationResult
                {
                    Error = exception.Message,
                };
                break;
            }
            case { } when exception is EntityNotFoundException:
            {
                statusCode = 404;
                response = new OperationResult
                {
                    Error = exception.Message,
                };
                break;
            }
            case { } when exception is AccessDeniedException:
            {
                statusCode = 403;
                response = new OperationResult
                {
                    Error = exception.Message,
                };
                break;
            }
            case { } when exception is UnauthorizedAccessException:
            {
                statusCode = 401;
                response = new OperationResult
                {
                    Error = "Не авторизован",
                };
                break;
            }
            case { } when exception is ArgumentException:
            {
                statusCode = 400;
                response = new OperationResult
                {
                    Error = exception.Message,
                };
                break;
            }
            default:
            {
                statusCode = 500;
                #if DEBUG
                response = new OperationResult
                {
                    Error = $"Ошибка: {exception}",
                };
                #else
                response = new OperationResult
                {
                    Error = $"Необработанная ошибка сервера",
                };
                #endif
                break;
            }
        }

        logger.LogError($"Api method {context.HttpContext.Request.Path} finished with code {statusCode} and error: " +
                        $"{JsonSerializer.Serialize(response)} : Exception: { exception.Message }");
        context.Result = new JsonResult(response) { StatusCode = statusCode };
        context.ExceptionHandled = true;
    }
}