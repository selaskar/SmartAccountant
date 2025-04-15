namespace SmartAccountant.Abstractions.Exceptions;

public class AccountException(string message, Exception? innerException) : Exception(message, innerException)
{
    public AccountException(string message) : this(message, null)
    { }
}
