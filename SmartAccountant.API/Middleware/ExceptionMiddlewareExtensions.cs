using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using SmartAccountant.Dtos.Response;

namespace SmartAccountant.API.Middleware;

internal static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(Handler);
    }

    private static void Handler(IApplicationBuilder builder)
    {
        builder.Run(Handler2);
    }

    private static async Task Handler2(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;

        var contextFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature == null)
            return;

        // No need to log errors here, as they are logged by default by ASP.NET Core.
        await httpContext.Response.WriteAsJsonAsync(new ErrorDetail()
        {
            Code = httpContext.Response.StatusCode,
            Error = "Internal Server Error.",
            Detail = contextFeature.Error.Message,
            Category = ErrorCategory.ServerError
        });
    }
}
