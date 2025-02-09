using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;

namespace FileStorage;

internal sealed class StorageService(BlobServiceClient client) : IStorageService
{
    /// <inheritdoc/>
    public async Task WriteToFile(string container, string filePath, Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(container);
            BlobClient blobClient = containerClient.GetBlobClient(filePath);

            if (await blobClient.ExistsAsync(cancellationToken))
                throw new StorageException($"The file ({filePath}) already exists in the container ({container}).");

            Response<BlobContentInfo> response = await blobClient.UploadAsync(stream, cancellationToken);

            Response rawResponse = response.GetRawResponse();

            if (rawResponse.IsError)
                throw new StorageException($"Upload failed with code {rawResponse.Status}");
        }
        catch (RequestFailedException ex)
        {
            throw new StorageException("An Azure blob storage-related exception occurred.", ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not StorageException)
        {
            throw new StorageException("An unexpected error occurred.", ex);
        }
    }
}
