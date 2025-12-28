namespace SmartAccountant.Abstractions.Exceptions;

public class TransactionException(TransactionErrors error, string message, Exception? innerException)
    : EnumException<TransactionErrors>(error, message, innerException)
{
    public TransactionException(TransactionErrors error, Exception? innerException) : this(error, error.ToString(), innerException)
    { }

    public TransactionException(TransactionErrors error, string message) : this(error, message, null)
    { }

    public TransactionException(TransactionErrors error) : this(error, error.ToString(), null)
    { }
}
