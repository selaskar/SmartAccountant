using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Dtos.Response;

namespace SmartAccountant.API.Filters;

internal sealed class EnumExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        Type? baseType = context.Exception.GetType().BaseType;

        if (baseType?.IsGenericType == false || baseType?.GetGenericTypeDefinition() != typeof(EnumException<>))
            return;

        var enumIndex = (int)(context.Exception as dynamic).Error;

        if (enumIndex == 0)
            return;

        context.Result = new BadRequestObjectResult(new ErrorDetail()
        {
            Code = enumIndex,
            Error = context.Exception.Message,
            Category = ErrorCategory.EnumException,
            Detail = context.Exception.GetAllMessages()
        });
    }
}
