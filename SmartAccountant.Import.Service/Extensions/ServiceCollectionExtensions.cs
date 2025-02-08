using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Validators;

namespace SmartAccountant.Import.Service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureImport(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<ImportStatementModel>, ImportStatementModelValidator>();

        services.AddScoped<IImportService, ImportService>();

        return services;
    }
}
