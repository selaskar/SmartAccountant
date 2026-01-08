using SmartAccountant.Shared.Enums.Errors;

namespace SmartAccountant.Abstractions.Exceptions;

public class AccountException(AccountErrors error, string message, Exception? innerException) 
    : EnumException<AccountErrors>(error, message, innerException)
{
    public AccountException(AccountErrors error, Exception? innerException) : this(error, error.ToString(), innerException)
    { }
}
