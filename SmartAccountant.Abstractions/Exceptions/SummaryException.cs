using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Abstractions.Exceptions;

public class SummaryException(SummaryErrors error, string message, Exception innerException)
    : EnumException<SummaryErrors>(error, message, innerException)
{
    public SummaryException(SummaryErrors error, Exception innerException)
        : this(error, error.ToString(), innerException)
    { }
}
