using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartAccountant.Dtos.Response;

namespace SmartAccountant.API.Filters;

internal sealed class ValidationExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (!(context?.Exception is not null and FluentValidation.ValidationException validationException))
            return;

        var category = ErrorCategory.ValidationException;

        if (int.TryParse(validationException.Errors.FirstOrDefault()?.ErrorCode, out int errorCode))
            category = ErrorCategory.EnumException;

        context.Result = new BadRequestObjectResult(new ErrorDetail()
        {
            Code = errorCode,
            Error = validationException.Message,
            Category = category
        });
    }
}
