using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Factories;
using SmartAccountant.Services.Parser.ParseStrategies;

namespace SmartAccountant.Services.Parser.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureParser(this IServiceCollection services)
    {
        // parse strategies
        services.AddSingleton<IStatementParseStrategy<DebitTransaction>, GarantiDebitStatementParseStrategy>();

        // factories
        services.AddSingleton<IStatementParseStrategyFactory, StatementParseStrategyFactory>();

        // parsers
        services.AddSingleton<ISpreadsheetParser, ExcelSpreadsheetParserService>();

        return services;
    }
}
