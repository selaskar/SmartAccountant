using Azure.Core;
using Azure.Storage;
using FileStorage.Options;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using SmartAccountant.Abstractions.Interfaces;

namespace FileStorage.Extensions;

public static class StorageServiceCollectionExtensions
{
    public static IServiceCollection ConfigureStorage(this IServiceCollection services, TokenCredential credential, AzureStorageOptions storageOptions)
    {
        services.AddAzureClients((clientBuilder) =>
        {
            clientBuilder.UseCredential(credential);

            clientBuilder.AddBlobServiceClient(new Uri(storageOptions.ServiceAddress))
                .ConfigureOptions(options => options.TransferValidation.Upload.ChecksumAlgorithm = StorageChecksumAlgorithm.Auto);
        });

        services.AddScoped<IStorageService, StorageService>();

        return services;
    }
}
