namespace SmartAccountant.Abstractions.Exceptions;

public class AuthenticationException(string message, Exception? innerException) : Exception(message, innerException)
{
    public AuthenticationException(string message) : this(message, null)
    { }
}
