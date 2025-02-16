using System.Diagnostics.CodeAnalysis;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Repositories.Core.DataContexts;
using SmartAccountant.Repositories.Core.Mappers;
using SmartAccountant.Repositories.Core.Options;

namespace SmartAccountant.Repositories.Core.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureCoreRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CoreDatabaseOptions>()
            .Bind(configuration.GetRequiredSection(CoreDatabaseOptions.Section), binderOptions => binderOptions.ErrorOnUnknownConfiguration = true)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<CoreDbContext>((services, builder) =>
        {
            CoreDatabaseOptions options = services.GetRequiredService<IOptions<CoreDatabaseOptions>>().Value;

            builder.UseAzureSql(options.ConnectionString);
        });

        services.AddAutoMapper(typeof(EntityToModelMappings));

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IStatementRepository, StatementRepository>();

        return services;
    }
}
