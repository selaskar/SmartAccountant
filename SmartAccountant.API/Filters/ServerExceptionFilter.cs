using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Dtos.Response;

namespace SmartAccountant.API.Filters;

internal sealed partial class ServerExceptionFilter(ILogger<ServerExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (!(context?.Exception is not null and ServerException exception))
            return;

        ServerExceptionOccurred(exception);

        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        context.Result = new ObjectResult(new ErrorDetail()
        {
            Code = context.HttpContext.Response.StatusCode,
            Error = "Internal Server Error.",
            Detail = exception.Message,
            Category = ErrorCategory.ServerError
        });
    }


    [LoggerMessage(Level = LogLevel.Error, Message = "A server exception occurred.")]
    private partial void ServerExceptionOccurred(Exception ex);
}
