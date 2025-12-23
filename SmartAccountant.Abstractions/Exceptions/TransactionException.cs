namespace SmartAccountant.Abstractions.Exceptions;

public class TransactionException(TransactionErrors error, string message, Exception? innerException)
    : EnumException<TransactionErrors>(error, message, innerException)
{
    public TransactionException(TransactionErrors error, Exception? innerException) : this(error, error.ToString(), innerException)
    { }
}

public enum TransactionErrors
{
    Unspecified = 0,
    AccountNotFound = 1,
    AccountDoesNotBelongToUser = 2,
    CannotFetchTransactionsOfAccount = 3,
    CannotUpdateDebitTransaction = 4,
    CannotUpdateCreditCardTransaction = 5,
}
