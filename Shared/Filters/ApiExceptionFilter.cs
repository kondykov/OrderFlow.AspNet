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
        ApiErrorResponse? response;

        switch (true)
        {
            case { } when exception is DuplicatedEntityException:
            {
                response = new ApiErrorResponse
                {
                    Code = 409,
                    Message = exception.Message,
                };
                break;
            }
            case { } when exception is EntityNotFoundException:
            {
                response = new ApiErrorResponse
                {
                    Code = 404,
                    Message = exception.Message,
                };
                break;
            }
            default:
            {
                response = new ApiErrorResponse
                {
                    Code = 500,
                    Message = $"Внутренняя ошибка сервера: {exception.Message}",
                };
                break;
            }
        }

        logger.LogError($"Api method {context.HttpContext.Request.Path} finished with code {response.Code} and error: " +
                        $"{JsonSerializer.Serialize(response)} : Exception: { exception }");
        context.Result = new JsonResult(response) { StatusCode = response.Code };
        context.ExceptionHandled = true;
    }
}