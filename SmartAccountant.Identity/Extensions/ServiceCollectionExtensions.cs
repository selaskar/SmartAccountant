using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Identity.Options;

namespace SmartAccountant.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static TokenCredential ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        //Built-in Validate() methods do not throw error, if the section is not provided at all.
        //GetRequiredSection() makes sure of that.
        services.AddOptions<IdentityOptions>()
            .Bind(configuration.GetRequiredSection(IdentityOptions.Section), binderOptions => binderOptions.ErrorOnUnknownConfiguration = true)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IAuthorizationService, AuthorizationService>();

        //Using this approach makes sure that option configuration is proper, before it is being used below.
        using IServiceScope scope = services.BuildServiceProvider().CreateScope();
        IdentityOptions identityOptions = scope.ServiceProvider.GetRequiredService<IOptions<IdentityOptions>>().Value;
        return identityOptions.CredentialType switch
        {
            CredentialType.VsCredential => new VisualStudioCredential(new VisualStudioCredentialOptions
            {
                TenantId = identityOptions.TenantId,
            }),
            CredentialType.AzureCli => new AzureCliCredential(new AzureCliCredentialOptions()
            {
                TenantId = identityOptions.TenantId,
            }),
            _ => new ManagedIdentityCredential(identityOptions.ClientId)
        };
    }
}
