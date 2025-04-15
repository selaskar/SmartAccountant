using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface IAccountService
{
    /// <exception cref="AccountException"/>
    /// <exception cref="AuthenticationException"/>
    IAsyncEnumerable<Account> GetAccountsOfUser();
}
