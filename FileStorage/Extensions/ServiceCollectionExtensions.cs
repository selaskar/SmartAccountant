using System.Diagnostics.CodeAnalysis;
using Azure.Core;
using Azure.Storage;
using FileStorage.Options;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using SmartAccountant.Abstractions.Interfaces;

namespace FileStorage.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureStorage(this IServiceCollection services, TokenCredential credential, AzureStorageOptions storageOptions)
    {
        services.AddAzureClients((clientBuilder) =>
        {
            clientBuilder.ConfigureDefaults(options => options.Retry.NetworkTimeout = TimeSpan.FromSeconds(5));

            clientBuilder.UseCredential(credential);

            clientBuilder.AddBlobServiceClient(new Uri(storageOptions.ServiceAddress))
                .ConfigureOptions(options => options.TransferValidation.Upload.ChecksumAlgorithm = StorageChecksumAlgorithm.Auto);
        });

        services.AddScoped<IStorageService, StorageService>();

        return services;
    }
}
