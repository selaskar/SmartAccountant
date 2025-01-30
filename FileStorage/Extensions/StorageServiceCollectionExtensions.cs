using Azure.Core;
using Azure.Storage.Blobs;
using FileStorage.Options;
using Microsoft.Extensions.DependencyInjection;
using SmartAccountant.Abstractions.Interfaces;

namespace FileStorage.Extensions;

public static class StorageServiceCollectionExtensions
{
    public static IServiceCollection RegisterBlobStorageClient(this IServiceCollection services, TokenCredential credential, AzureStorageOptions storageOptions)
    {
        services.AddScoped(serviceProvider => new BlobServiceClient(new Uri(storageOptions.ServiceAddress), credential));
        services.AddScoped<IStorageService, StorageService>();

        return services;
    }
}
