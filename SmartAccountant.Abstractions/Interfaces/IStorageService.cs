using SmartAccountant.Abstractions.Exceptions;

namespace SmartAccountant.Abstractions.Interfaces;

public interface IStorageService
{
    /// <exception cref="StorageException"/>
    /// <exception cref="OperationCanceledException"/>
    Task WriteToFile(string container, string filePath, Stream stream, CancellationToken cancellationToken);
}
