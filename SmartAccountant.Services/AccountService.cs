using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Interfaces;
using SmartAccountant.Models;
using SmartAccountant.Repositories.Core.Abstract;
using SmartAccountant.Services.Resources;

namespace SmartAccountant.Services;

internal class AccountService(IAccountRepository accountRepository, IAuthorizationService authorizationService) : IAccountService
{
    /// <inheritdoc/>
    public IAsyncEnumerable<Account> GetAccountsOfUser()
    {
        Guid userId = authorizationService.UserId;

        try
        {
            return accountRepository.GetAccountsOfUser(userId);
        }
        catch (Exception ex)
        {
            throw new AccountException(Messages.CannotFetchAccountsOfUser, ex);
        }
    }
}
