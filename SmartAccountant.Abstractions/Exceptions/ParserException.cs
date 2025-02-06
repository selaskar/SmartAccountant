namespace SmartAccountant.Abstractions.Exceptions;

public class ParserException(string message, Exception? innerException) : Exception(message, innerException)
{
    public ParserException(string message) : this(message, null)
    {

    }
}
