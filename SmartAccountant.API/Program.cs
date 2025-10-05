using System.IdentityModel.Tokens.Jwt;
using Azure.Core;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using FileStorage.Extensions;
using FileStorage.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using SmartAccountant.API.Filters;
using SmartAccountant.API.Mappers;
using SmartAccountant.Identity.Extensions;
using SmartAccountant.Import.Service.Extensions;
using SmartAccountant.Repositories.Core.Extensions;
using SmartAccountant.Services.Extensions;
using SmartAccountant.Services.Parser.Extensions;

namespace SmartAccountant.API;

internal sealed class Program
{
    internal static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        ConfigureAuthentication(builder);

        //Runtime handles the exceptions thrown by the cancellation token that is passed to the action.
        //No need for custom exception filter.
        builder.Services
            .AddControllers(options =>
            {
                options.Filters.Add<ValidationExceptionFilter>();
                options.Filters.Add<AuthenticationExceptionFilter>();
            })
            .AddJsonOptions(options => options.JsonSerializerOptions.AllowTrailingCommas = true);

        ConfigureDocumentation(builder);

        TokenCredential credential = builder.Services.ConfigureIdentity(builder.Configuration);

        ConfigureLogging(builder, credential);

        builder.Services.ConfigureStorage(credential, GetOptions<AzureStorageOptions>(builder.Configuration, AzureStorageOptions.Section));

        builder.Services.ConfigureCoreRepository(builder.Configuration);

        builder.Services.ConfigureImport();

        builder.Services.ConfigureParser();

        builder.Services.AddAutoMapper(typeof(RequestResponseMappings));

        builder.Services.ConfigureCoreServices();

        BuildAndRun(builder);
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        // This is required to be instantiated before the OpenIdConnectOptions starts getting configured.
        // By default, the claims mapping will map claim names in the old format to accommodate older SAML applications.
        // For instance, 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' instead of 'roles' claim.
        // This flag ensures that the ClaimsIdentity claims collection will be built from the claims in the token
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;


        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetRequiredSection("AzureAd"));

        //Used when authorize attribute doesn't specify a policy
        //Doesn't enforce a role, since it is not straightforward to automatically assign 
        //application roles to new users during self-registration.
        AuthorizationPolicy defaultPolicy = new AuthorizationPolicyBuilder()
            .RequireScope(AppScopes.Statement.ToString())
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
        builder.Services.AddOpenTelemetry().UseAzureMonitor(options =>
        {
            options.ConnectionString = builder.Configuration.GetValue<string>("AzureMonitor:ConnectionString");

            options.Credential = credential;

            options.Retry.NetworkTimeout = TimeSpan.FromSeconds(5);
        });
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
