using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Dtos.Response;

namespace SmartAccountant.API.Middleware;

internal static class ExceptionMiddlewareExtensions
{
    internal static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(static (IApplicationBuilder builder) => builder.Run(Handler));
    }

    private static async Task Handler(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;

        var contextFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature == null)
            return;

        // No need to log errors here, as they are logged by ExceptionHandlerMiddleware. See app settings.
        await httpContext.Response.WriteAsJsonAsync(new ErrorDetail()
        {
            Code = httpContext.Response.StatusCode,
            Error = "Internal Server Error",
            Category = ErrorCategory.ServerError,
            Detail = contextFeature.Error.GetAllMessages(),
        });
    }
}
