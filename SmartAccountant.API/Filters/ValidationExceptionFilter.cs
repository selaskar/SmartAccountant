using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartAccountant.Filters;

internal sealed class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context?.Exception is null or not ValidationException)
            return;

        context.Result = new BadRequestObjectResult(context.Exception.Message);
    }
}
