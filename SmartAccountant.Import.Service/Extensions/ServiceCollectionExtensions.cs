using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Import.Service.Abstract;
using SmartAccountant.Import.Service.Factories;
using SmartAccountant.Import.Service.Helpers;
using SmartAccountant.Import.Service.Validators;

namespace SmartAccountant.Import.Service.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureImport(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<DebitStatementImportModel>, DebitStatementImportModelValidator>();

        services.AddSingleton<IValidator<CreditCardStatementImportModel>, CreditCardStatementImportModelValidator>();

        services.AddSingleton<IValidator<MultipartStatementImportModel>, MultipartStatementImportModelValidator>();

        services.AddSingleton<IFileTypeValidator, FileTypeValidator>();

        services.AddSingleton<IStatementFactory, StatementFactory>();

        services.AddKeyedScoped<IImportService, DebitImportService>(nameof(ImportableStatementTypes.Debit));

        services.AddKeyedScoped<IImportService, CreditCardImportService>(nameof(ImportableStatementTypes.CreditCard));

        services.AddKeyedScoped<IImportService, MultipartCreditCardImportService>(nameof(ImportableStatementTypes.Multipart));

        return services;
    }
}
