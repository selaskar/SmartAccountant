using SmartAccountant.Abstractions.Models.Request;

namespace SmartAccountant.Import.Service.Abstract;

internal interface IFileTypeValidator
{
    /// <remarks>The method leaves the position of file stream intact.</remarks>
    /// <exception cref="OperationCanceledException" />
    Task<bool> IsValidFile(ImportFile file, CancellationToken cancellationToken);
}
