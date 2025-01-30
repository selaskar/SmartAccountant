namespace SmartAccountant.Abstractions.Exceptions;

public class StorageException(string? message, Exception? innerException) : Exception(message, innerException)
{
}
