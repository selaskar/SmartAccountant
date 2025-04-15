using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SmartAccountant.Abstractions.Interfaces;

namespace SmartAccountant.Services.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureCoreServices(this IServiceCollection services)
    {
        return services.AddScoped<IAccountService, AccountService>();
    }
}
