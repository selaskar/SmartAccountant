namespace SmartAccountant.Abstractions.Exceptions;

public class EnumException<T>(T error, string message, Exception? inner) : Exception(message, inner)
    where T : Enum
{
    public EnumException(T error, string message) : this(error, message, null)
    { }

    public EnumException(T error) : this(error, error.ToString(), null)
    { }

    public T Error { get; } = error;
}
