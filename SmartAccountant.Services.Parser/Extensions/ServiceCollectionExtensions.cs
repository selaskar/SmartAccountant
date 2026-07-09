using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Factories;

namespace SmartAccountant.Services.Parser.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureParser(this IServiceCollection services)
    {
        // factories
        services.AddSingleton<ISpreadsheetParseStrategyFactory, SpreadsheetParseStrategyFactory>();

        // parsers
        services.AddSingleton<IStatementParser, ExcelSpreadsheetParserService>();
        //services.AddSingleton<ISpreadsheetParser, ExcelSpreadsheetParserService>();

        return services;
    }
}
