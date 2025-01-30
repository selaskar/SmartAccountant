using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;

namespace FileStorage;

public class StorageService(BlobServiceClient client) : IStorageService
{
    /// <inheritdoc/>
    public async Task WriteToFile(string container, string filePath, Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(container);

            BlobClient blobClient = containerClient.GetBlobClient(filePath);

            Response<BlobContentInfo> response = await blobClient.UploadAsync(stream, cancellationToken);
            //TODO: handle file name collision.
        }
        catch (RequestFailedException ex)
        {
            throw new StorageException("An Azure blob storage-related exception occurred.", ex);
        }
        catch (Exception ex)
        {
            throw new StorageException("An unexpected error occurred.", ex);
        }
    }
}
