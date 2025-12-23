using Microsoft.AspNetCore.Mvc;
using SmartAccountant.Dtos.Response;

namespace SmartAccountant.API.Extensions;

public static class ControllerExtensions
{
    public static IActionResult EnumResult<TEnum>(this Controller _, TEnum error) where TEnum : Enum
    {
        if (default(TEnum)!.Equals(error))
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);

        return new BadRequestObjectResult(new ErrorDetail
        {
            Code = error.GetHashCode(),
            Error = error.ToString()
        });
    }
}
