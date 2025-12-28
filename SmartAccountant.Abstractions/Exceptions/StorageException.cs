namespace SmartAccountant.Abstractions.Exceptions;

public class StorageException(StorageErrors error, string message, Exception? innerException)
    : EnumException<StorageErrors>(error, message, innerException)
{
    public StorageException(StorageErrors error, Exception innerException)
        : this(error, error.ToString(), innerException)
    { }

    public StorageException(StorageErrors error, string message)
        : this(error, message, null)
    { }
}
