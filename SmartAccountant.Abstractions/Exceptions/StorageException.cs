namespace SmartAccountant.Abstractions.Exceptions;

public class StorageException(string? message, Exception? innerException) : Exception(message, innerException)
{
    public StorageException(string? message) : this(message, null)
    { }
}
