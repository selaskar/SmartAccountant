using SmartAccountant.Models.Request;

namespace SmartAccountant.Import.Service.Abstract;

internal interface IFileTypeValidator
{
    /// <remarks>The method leaves the position of file stream intact.</remarks>
    /// <exception cref="OperationCanceledException" />
    /// <exception cref="ArgumentException" />
    /// <exception cref="ArgumentNullException" />
    Task<bool> IsValidFile(ImportFile file, CancellationToken cancellationToken);
}
