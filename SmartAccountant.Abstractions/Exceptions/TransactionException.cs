namespace SmartAccountant.Abstractions.Exceptions;

public class TransactionException(string message, Exception? innerException) : Exception(message, innerException)
{
}
