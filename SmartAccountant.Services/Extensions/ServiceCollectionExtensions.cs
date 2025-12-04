using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Models;
using SmartAccountant.Services.Validators;

namespace SmartAccountant.Services.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<DebitTransaction>, DebitTransactionValidator>();
        services.AddSingleton<IValidator<CreditCardTransaction>, CreditCardTransactionValidator>();

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<ISummaryService, SummaryService>();
        services.AddScoped<ITransactionService, TransactionService>();

        return services;
    }
}
