namespace SmartAccountant.Abstractions.Exceptions;

public class AccountException(string message, Exception? innerException) : Exception(message, innerException)
{
}
