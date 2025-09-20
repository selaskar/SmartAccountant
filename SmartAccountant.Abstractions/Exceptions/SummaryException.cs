namespace SmartAccountant.Abstractions.Exceptions;

public class SummaryException(string message, Exception? innerException) : Exception(message, innerException)
{
}
