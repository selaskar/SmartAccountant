using System.Text;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileStorage.Resources;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Shared.Enums.Errors;

namespace FileStorage;

internal sealed class StorageService(BlobServiceClient client) : IStorageService
{
    private static readonly CompositeFormat UploadFailed = CompositeFormat.Parse(Messages.UploadFailed);
    private static readonly CompositeFormat FileAlreadyExists = CompositeFormat.Parse(Messages.FileAlreadyExists);
    private static readonly CompositeFormat CannotWriteToFile = CompositeFormat.Parse(Messages.CannotWriteToFile);
  
    /// <inheritdoc/>
    public async Task WriteToFile(string container, string filePath, Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(container);
            BlobClient blobClient = containerClient.GetBlobClient(filePath);

            if (await blobClient.ExistsAsync(cancellationToken))
                throw new StorageException(StorageErrors.FileAlreadyExists, FileAlreadyExists.FormatMessage(filePath, container));

            Response<BlobContentInfo> response = await blobClient.UploadAsync(stream, cancellationToken);

            Response rawResponse = response.GetRawResponse();

            if (rawResponse.IsError)
                throw new StorageException(StorageErrors.UploadFailed, UploadFailed.FormatMessage(rawResponse.Status));
        }
        catch (RequestFailedException ex)
        {
            throw new StorageException(StorageErrors.AzureStorageError, ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not StorageException)
        {
            throw new ServerException(CannotWriteToFile.FormatMessage(filePath, container), ex);
        }
    }
}
