namespace SmartAccountant.Abstractions.Exceptions;

public class AccountException(AccountErrors error, string message, Exception? innerException) 
    : EnumException<AccountErrors>(error, message, innerException)
{
    public AccountException(AccountErrors error, Exception? innerException) : this(error, error.ToString(), innerException)
    { }
}

public enum AccountErrors
{
    Unspecified = 0,
    CannotFetchAccountsOfUser = 1,
}
