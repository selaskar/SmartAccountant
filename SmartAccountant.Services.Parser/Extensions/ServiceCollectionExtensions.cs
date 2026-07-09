using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Factories;
using SmartAccountant.Services.Parser.ParseStrategies;

namespace SmartAccountant.Services.Parser.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureParser(this IServiceCollection services)
    {
        // factories
        //services.AddSingleton<IStatementParseStrategyFactory, StatementParseStrategyFactory>();
        services.AddSingleton<ISpreadsheetParseStrategyFactory, StatementParseStrategyFactory>();

        // parsers
        services.AddSingleton<IStatementParser, ExcelSpreadsheetParserService>();
        services.AddSingleton<ISpreadsheetParser, ExcelSpreadsheetParserService>();

        return services;
    }
}
