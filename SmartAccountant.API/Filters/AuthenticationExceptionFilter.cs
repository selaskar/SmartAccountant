using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartAccountant.Abstractions.Exceptions;

namespace SmartAccountant.API.Filters;

internal sealed class AuthenticationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context?.Exception is null or not AuthenticationException)
            return;

        context.Result = new UnauthorizedObjectResult(context.Exception.Message);
    }
}
