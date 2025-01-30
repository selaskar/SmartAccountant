using Azure.Core;
using FileStorage.Extensions;
using FileStorage.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using SmartAccountant.Identity.Extensions;

namespace SmartAccountant;

internal sealed class Program
{
    internal static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        ConfigureAuthentication(builder);

        //Runtime handles the exceptions thrown by the cancellation token that is passed to the action.
        //No need for custom exception filter.
        builder.Services
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.AllowTrailingCommas = true);

        ConfigureDocumentation(builder);

        TokenCredential credential = builder.Services.ConfigureIdentity(builder.Configuration);

        builder.Services.RegisterBlobStorageClient(credential, GetOptions<AzureStorageOptions>(builder.Configuration, AzureStorageOptions.Section));
        
        ConfigureLogging(builder, credential);

        BuildAndRun(builder);
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetRequiredSection("AzureAd"));

        //Used when authorize attribute doesn't specify a policy
        AuthorizationPolicy defaultPolicy = new AuthorizationPolicyBuilder()
            .RequireScope(AppScopes.Statement.ToString())
            .RequireRole(AppRoles.RegularUser.ToString(), AppRoles.Developer.ToString())
            .Build();

        //Used when a controller doesn't specify an authorize attribute.
        AuthorizationPolicy fallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        builder.Services.AddAuthorizationBuilder()
            .SetDefaultPolicy(defaultPolicy)
            .SetFallbackPolicy(fallbackPolicy)
            .AddPolicy(AuthPolicies.ApiConsumer.ToString(), builder => builder.RequireRole(AppRoles.Developer.ToString()));
    }

    private static void ConfigureDocumentation(WebApplicationBuilder builder)
    {
        //Note that adding XML documentation to API description is not yet supported as of .NET 9.
        builder.Services.AddOpenApi();
    }

    private static void ConfigureLogging(WebApplicationBuilder builder, TokenCredential credential)
    {
        builder.Logging.AddApplicationInsights(configureTelemetryConfiguration: config =>
        {
            config.ConnectionString = builder.Configuration.GetValue<string>("ApplicationInsights:ConnectionString");

            if (builder.Environment.IsDevelopment())
                config.SetAzureTokenCredential(credential);
        }, configureApplicationInsightsLoggerOptions: option => { });
    }

    private static void BuildAndRun(WebApplicationBuilder builder)
    {
        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi().RequireAuthorization(AuthPolicies.ApiConsumer.ToString());
            
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }


    private static T GetOptions<T>(ConfigurationManager configuration, string section)
    {
        T options = configuration.GetRequiredSection(section)
            .Get<T>(options => options.ErrorOnUnknownConfiguration = true)
            ?? throw new InvalidOperationException($"Settings for {typeof(T).Name} were not properly configured.");

        return options;
    }

    internal enum AppScopes
    {
        Statement = 0
    }

    internal enum AuthPolicies
    {
        Default = 0,
        ApiConsumer = 1
    }

    internal enum AppRoles
    {
        RegularUser = 0,
        Developer = 1
    }
}
