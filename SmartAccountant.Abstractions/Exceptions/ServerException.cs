namespace SmartAccountant.Abstractions.Exceptions;

public class ServerException(string message, Exception inner) : Exception(message, inner)
{
}
