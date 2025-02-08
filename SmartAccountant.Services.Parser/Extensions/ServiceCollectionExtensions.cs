using Microsoft.Extensions.DependencyInjection;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Factories;
using SmartAccountant.Services.Parser.ParseStrategies;

namespace SmartAccountant.Services.Parser.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureParser(this IServiceCollection services)
    {
        // parse strategies
        services.AddSingleton<IStatementParseStrategy, GarantiStatementParseStrategy>();

        // factories
        services.AddSingleton<IStatementParseStrategyFactory, StatementParseStrategyFactory>();

        // parsers
        services.AddSingleton<ISpreadsheetParser, ExcelSpreadsheetParserService>();

        return services;
    }
}
