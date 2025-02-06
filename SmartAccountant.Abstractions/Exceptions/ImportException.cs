namespace SmartAccountant.Abstractions.Exceptions;

public class ImportException(string message, Exception? innerException) : Exception(message, innerException)
{
    public ImportException(string message) : this(message, null)
    { }
}
