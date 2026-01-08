using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Abstractions.Exceptions;

public class ImportException(ImportErrors error, string message, Exception? innerException)
    : EnumException<ImportErrors>(error, message, innerException)
{
    public ImportException(ImportErrors error, string message) : this(error, message, null)
    { }

    public ImportException(ImportErrors error, Exception? innerException) : this(error, error.ToString(), innerException)
    { }

    public ImportException(ImportErrors error) : this(error, error.ToString(), null)
    {
        Data["AccountId"] = Guid.Empty;
    }
}
