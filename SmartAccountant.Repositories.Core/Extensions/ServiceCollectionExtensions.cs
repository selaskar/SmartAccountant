using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

            builder.UseAzureSql(options.ConnectionString,
                builder => builder.CommandTimeout(5).ExecutionStrategy((deps) => new NonRetryingExecutionStrategy(deps)));
        }, ServiceLifetime.Scoped);

        services.AddAutoMapper(cfg => cfg.AddProfile<EntityToModelMappings>());

        //Note that, in order unit of work to work correctly, db context and repositories must be registered as scoped.
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IStatementRepository, StatementRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }
}
