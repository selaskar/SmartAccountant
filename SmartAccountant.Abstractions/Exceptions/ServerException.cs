namespace SmartAccountant.Abstractions.Exceptions;

/// <remarks>This exception isn't supposed to be nested into other exceptions. 
/// It will be directly caught by the corresponding action filter.</remarks>
public class ServerException(string message, Exception inner) : Exception(message, inner)
{
}
