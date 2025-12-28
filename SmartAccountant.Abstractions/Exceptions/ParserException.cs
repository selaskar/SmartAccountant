using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Abstractions.Exceptions;

public class ParserException(ParserErrors error, string message, Exception? innerException)
    : EnumException<ParserErrors>(error, message, innerException)
{
    public ParserException(ParserErrors error, string message) : this(error, message, null)
    {
    }

    public ParserException(ParserErrors error, Exception ex) : this(error, error.ToString(), ex)
    {
    }

    public ParserException(ParserErrors error) : this(error, error.ToString(), null)
    {
    }
}
